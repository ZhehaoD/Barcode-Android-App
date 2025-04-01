using Camera.MAUI.ZXing;
using Camera.MAUI;

namespace Barcode_enabled_Android_App;

public partial class BarcodeScanning : ContentPage
{
	public BarcodeScanning()
	{
		InitializeComponent();

        cameraView.BarCodeDecoder = new ZXingBarcodeDecoder();
        cameraView.BarCodeOptions = new BarcodeDecodeOptions
        {
            AutoRotate = true,
            PossibleFormats = { BarcodeFormat.QR_CODE, BarcodeFormat.All_1D },
            ReadMultipleCodes = false,
            TryHarder = false,
            TryInverted = true
        };
        cameraView.BarCodeDetectionEnabled = true;
    }

    private void cameraView_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
    {
        Shell.Current.GoToAsync(
            $"..?format={args.Result[0].BarcodeFormat}&barcode={args.Result[0].Text}");
    }

    private void cameraView_CamerasLoaded(object sender, EventArgs e)
    {
        if (cameraView.Cameras.Count > 0)
        {
            cameraView.Camera = cameraView.Cameras.First();
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await cameraView.StopCameraAsync();
                await cameraView.StartCameraAsync();
            });
        }
    }
}