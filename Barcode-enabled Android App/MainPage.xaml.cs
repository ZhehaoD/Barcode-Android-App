using System.Diagnostics;
using System.Text.Json;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
namespace Barcode_enabled_Android_App
{
    public partial class MainPage : ContentPage
    {
        HttpClient client = new HttpClient();
        private void Network_test()
        {
            Debug.WriteLine("Network test start");
            // Try to get something from Internet
            try
            {
                var resp = client.GetAsync("https://freeipapi.com/api/json").Result;
                Debug.WriteLine("Network test ok. " + resp.Content.ReadAsStringAsync().Result);
                LabelHttpResponse.Text = "Network ready";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Network test fail. " + ex.Message);
                LabelHttpResponse.Text = "Network may not ready";
            }
        }
        public MainPage()
        {
            InitializeComponent();

            Network_test();
        }
        private async Task Show_Toast(string message)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            ToastDuration duration = ToastDuration.Short;
            double fontSize = 14;
            var toast = Toast.Make(message, duration, fontSize);
            await toast.Show(cancellationTokenSource.Token);
        }

        private void ResetFoodDetail()
        {
            // Default contents
            LabelBrandName.Text = "Brand Name: ";
            LabelProductName.Text = "Product Name: ";
            LabelIngredients.Text = "Ingredients: ";
            LabelCategories.Text = "Categories: ";
            LabelAllergens.Text = "Allergens: None";

            LabelMessage.Text = string.Empty;
            LabelMessage.TextColor = Colors.Black;
            ImageCover.Source = ImageSource.FromFile("image_coming_soon.png");
        }
        private void ParseFoodJSON(string json)
        {
            // Convert http response content to JSON object
            using (var jsonDocument = JsonDocument.Parse(json))
            {
                var rootElement = jsonDocument.RootElement;
                Console.WriteLine(rootElement.ToString());
                // try to get title
                if (rootElement.TryGetProperty("product", out var product))
                {
                    if(product.TryGetProperty("brands",out var brands))
                    {
                        LabelBrandName.Text += brands.ToString();
                    }
                    if (product.TryGetProperty("abbreviated_product_name", out var ProductName))
                    {
                        LabelProductName.Text += ProductName.ToString();
                    }
                    if (LabelProductName.Text == "Product Name: ")
                    {
                        if (product.TryGetProperty("generic_name", out var generi))
                        {
                           LabelProductName.Text += generi.ToString();
                        }
                    }
                    if (product.TryGetProperty("ingredients_original_tags", out var Ingredient))
                    {
                        LabelIngredients.Text += Ingredient.ToString();
                    }
                    if (product.TryGetProperty("categories_hierarchy", out var categories))
                    {
                        LabelCategories.Text = "Allergens: "+ categories.ToString();
                    }
                    if (product.TryGetProperty("allergens", out var allergens))
                    {
                        LabelAllergens.Text += allergens.ToString();
                    }
                    if (product.TryGetProperty("image_url", out var image))
                    {
                        ImageCover.Source = ImageSource.FromUri(new Uri(image.ToString()));
                    }

                }
            }
            
        }
        
        private async void FindBtn_Clicked(object sender, EventArgs e)
        {
            if (EntryBarcode.Text.Trim().Length == 0)
            {
                // No ISBN number is entered
                await Show_Toast("Please enter an Barcode number");
                return;
            }
            ResetFoodDetail();
            try
            {
                await Show_Toast("Querying food information");
                string ApiUrl = $"https://world.openfoodfacts.org/api/v0/product/{EntryBarcode.Text.Trim()}.json";
                var resp = await client.GetStringAsync(ApiUrl);
                LabelHttpResponse.Text = resp;
                if (resp.Length < 13)
                {
                    LabelMessage.Text = "Food not found";
                    return;
                }
                ParseFoodJSON(resp);
            }
            catch (Exception ex)
            {
                LabelHttpResponse.Text =
               "Querying book information error. " + ex.Message;
                Debug.WriteLine(LabelHttpResponse.Text);
            }
        }
    }
}
