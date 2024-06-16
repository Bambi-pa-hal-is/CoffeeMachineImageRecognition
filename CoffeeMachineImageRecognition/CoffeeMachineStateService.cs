using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMachineImageRecognition
{
    public class CoffeeMachineStateService
    {
        private BeverageEnum _currentState;
        private readonly CoffeeMachineApiClient _client;
        private const double ConfidenceThreshold = 0.8;

        public CoffeeMachineStateService(CoffeeMachineApiClient client)
        {
            _client = client;
            _currentState = BeverageEnum.Unknown;
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
                await _client.Update(detectedBeverage);
            }

            // Update the current state to the detected beverage
            _currentState = detectedBeverage;
        }
    }
}
