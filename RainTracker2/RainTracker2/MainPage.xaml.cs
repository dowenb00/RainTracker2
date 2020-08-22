using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.GoogleMaps;


namespace RainTracker2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        List<Location> locs;
        WebHandler webHandler;
        Xamarin.Essentials.Location currentLocation;
        Geocoder geocoder;

        public MainPage()
        {
            InitializeComponent();
            locs = new List<Location>();
            webHandler = new WebHandler();
            GetCurrentLocation();
            LoadMap();
            geocoder = new Geocoder();
        }

        public async void ShowLocationDialog(Object sender, EventArgs e)
        {
            var position = ((MapClickedEventArgs)e).Point;

            IEnumerable<string> addresses = await geocoder.GetAddressesForPositionAsync(position);
            var pos = await geocoder.GetPositionsForAddressAsync("China");
            string defaultAddress = addresses.FirstOrDefault();

            if (await DisplayAlert("", "Would you like to start tracking rainfall for " + defaultAddress + "?", "Yes", "No"))
            {
                webHandler.AddLocation(position.Latitude, position.Longitude);
            }
        }

        public async void GetCurrentLocation()
        {
            currentLocation = await Geolocation.GetLastKnownLocationAsync();
        }

        public async void LoadMap()
        {
            locs = await webHandler.GetLocationData();
            foreach (Location loc in locs)
            {
                Pin spotPin = new Pin()
                {
                    Label = loc.LocationName + loc.GetSpanTotal(startDatePicker.Date, endDatePicker.Date),
                    Position = new Position(loc.Latitude, loc.Longitude),
                    Address = loc.LocationKey
                };
                RainMap.Pins.Add(spotPin);
            }
            RainMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(currentLocation.Latitude, currentLocation.Longitude), Distance.FromKilometers(100)));
            RainMap.PinClicked += async (sender, e) =>
            {
                DisplayLocationInfo(e.Pin.Address);
            };
        }

        public async void DisplayLocationInfo(string locKey)

        {
            foreach (Location loc in locs)
            {
                if (loc.LocationKey == locKey)
                {
                    double total = 0;
                    string message = loc.LocationName + ":";
                    foreach (DailyTotal dt in loc.Totals)
                    {
                        if (dt.Date >= startDatePicker.Date && dt.Date <= endDatePicker.Date)
                        {
                            total += dt.inches;
                            message += "\n" + dt.Date.ToString("dddd, dd MMMM yyyy") + ": " + dt.inches + " inches";
                        }
                    }
                    message += "\n\n" + "Total: " + total + " inches";
                    await DisplayAlert("Totals", message, "Ok");
                }
            }
        }
        void ShowTotals(Object sender, EventArgs e)
        {

        }
    }
}