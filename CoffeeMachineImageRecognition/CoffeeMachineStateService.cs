using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMachineImageRecognition
{
    public class CoffeeMachineStateService
    {
        private BeverageEnum _currentState;
        private readonly CoffeeMachineApiClient _client;
        private const double ConfidenceThreshold = 0.9;
        private const int DetectionThreshold = 3;
        private int _consecutiveDetections = 0;
        private bool _menuDetected = false;

        private Dictionary<BeverageEnum, BeverageQuota> _beverageQuotas;

        public CoffeeMachineStateService(CoffeeMachineApiClient client)
        {
            _client = client;
            _currentState = BeverageEnum.Unknown;
            _beverageQuotas = new Dictionary<BeverageEnum, BeverageQuota>
            {
                { BeverageEnum.Coffee, new BeverageQuota(1) },
                { BeverageEnum.HotWater, new BeverageQuota(1) },
                { BeverageEnum.CafeAuLait, new BeverageQuota(1) },
                { BeverageEnum.Chocodream, new BeverageQuota(1) },
                { BeverageEnum.Espresso, new BeverageQuota(1) },
                { BeverageEnum.LatteMachiato, new BeverageQuota(1) },
                { BeverageEnum.HotChocolate, new BeverageQuota(1) },
                { BeverageEnum.CaffeLatte, new BeverageQuota(1) },
                { BeverageEnum.Cappuccino, new BeverageQuota(1) },
                // Add other beverages with specific quotas
                { BeverageEnum.Menu, new BeverageQuota(5) }, 
                { BeverageEnum.Unknown, new BeverageQuota(5) },
            };
        }

        public async Task ProcessBeverageEnum(BeverageEnum detectedBeverage, double confidence)
        {
            if (confidence < ConfidenceThreshold)
            {
                // Confidence below threshold, reset counter and do nothing
                _consecutiveDetections = 0;
                return;
            }

            if (detectedBeverage == BeverageEnum.Unknown)
            {
                // Detected unknown, reset counter and do nothing
                _consecutiveDetections = 0;
                return;
            }

            if (detectedBeverage == BeverageEnum.Menu)
            {
                // Detected menu, update the state to Menu and reset the counter
                _currentState = BeverageEnum.Menu;
                _consecutiveDetections = 0;
                _menuDetected = true;
                return;
            }

            if (_currentState == detectedBeverage)
            {
                // Increment the counter if the same beverage is detected consecutively
                _consecutiveDetections++;
            }
            else
            {
                // Reset the counter if a different beverage is detected
                _consecutiveDetections = 0;
                _currentState = detectedBeverage;
            }

            if (_consecutiveDetections >= DetectionThreshold && _menuDetected)
            {
                // State changes from menu to something else, send the enum to the API
                await _client.Update(detectedBeverage);
                _currentState = detectedBeverage;
                // Reset the menu detection flag after sending the update
                _menuDetected = false;
            }
        }

        public async Task UploadImage(BeverageEnum detectedBeverage, Mat image)
        {
            try
            {
                var uploadUrl = await _client.GetUploadUrl(detectedBeverage);
                await _client.UploadImage(image, uploadUrl);
            }
            catch (Exception ex)
            {
            }
        }

        public bool CheckAndUpdateQuota(BeverageEnum beverage)
        {
            if(beverage == BeverageEnum.Menu)
            {
                return false;
            }
            if (_beverageQuotas.TryGetValue(beverage, out var quota))
            {
                return quota.ConsumeQuota();
            }
            return false; // No quota found means no upload allowed
        }

        public void ResetQuotas()
        {
            foreach (var quota in _beverageQuotas.Values)
            {
                quota.ResetQuota();
            }
        }
    }
}
