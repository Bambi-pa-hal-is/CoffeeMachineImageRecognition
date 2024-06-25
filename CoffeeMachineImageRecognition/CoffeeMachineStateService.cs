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

        private Dictionary<BeverageEnum, BeverageQuota> _beverageQuotas;

        public CoffeeMachineStateService(CoffeeMachineApiClient client)
        {
            _client = client;
            _currentState = BeverageEnum.Unknown;
            _beverageQuotas = new Dictionary<BeverageEnum, BeverageQuota>
            {
                { BeverageEnum.Coffee, new BeverageQuota(50) },
                { BeverageEnum.HotWater, new BeverageQuota(50) },
                { BeverageEnum.CafeAuLait, new BeverageQuota(50) },
                { BeverageEnum.Chocodream, new BeverageQuota(50) },
                { BeverageEnum.Espresso, new BeverageQuota(50) },
                { BeverageEnum.LatteMachiato, new BeverageQuota(50) },
                { BeverageEnum.HotChocolate, new BeverageQuota(50) },
                { BeverageEnum.CaffeLatte, new BeverageQuota(50) },
                { BeverageEnum.Cappuccino, new BeverageQuota(50) },
                // Add other beverages with specific quotas
                { BeverageEnum.Menu, new BeverageQuota(5) }, 
                { BeverageEnum.Unknown, new BeverageQuota(5) },
            };
        }

        public async Task ProcessBeverageEnum(BeverageEnum detectedBeverage, double confidence)
        {


            if (confidence < ConfidenceThreshold)
            {
                // Confidence below threshold, do nothing
                return;
            }

            if (detectedBeverage == BeverageEnum.Unknown)
            {
                // Detected unknown, do nothing
                return;
            }

            if (detectedBeverage == BeverageEnum.Menu)
            {
                // Detected menu, update the state to Menu
                _currentState = BeverageEnum.Menu;
                return;
            }

            if (_currentState == BeverageEnum.Menu && detectedBeverage != BeverageEnum.Menu)
            {
                // State changes from menu to something else, send the enum to the API
                Console.WriteLine("Sending input " + detectedBeverage.ToString());
                await _client.Update(detectedBeverage);
            }

            // Update the current state to the detected beverage
            _currentState = detectedBeverage;
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
