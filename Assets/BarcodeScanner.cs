using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEngine.UI;
using Vuforia;

using ZXing;
using ZXing.QrCode;
using ZXing.Common;

[AddComponentMenu("System/BarcodeScanner")]
public class BarcodeScanner : MonoBehaviour {

	public Text OutputText;

	private bool cameraInitialized;

	private BarcodeReader barCodeReader;

	// Use this for initialization
	void Start()
	{     
		//Barcode reader instance
		barCodeReader = new BarcodeReader();
		StartCoroutine(InitializeCamera());
	}

	//Connect the camera with the barcode scanner
	private IEnumerator InitializeCamera()
	{
		// Waiting a little seem to avoid the Vuforia's crashes.
		yield return new WaitForSeconds(1.25f);

		var isFrameFormatSet = CameraDevice.Instance.SetFrameFormat(Vuforia.Image.PIXEL_FORMAT.RGB888, true);

		// Force autofocus.
		var isAutoFocus = CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
		if (!isAutoFocus)
		{
			CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
		}
		cameraInitialized = true;
	}

	// Update is called once per frame
	void Update()
	{
		// Exit the app when the 'back' button is pressed.
		if (Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}

		if (cameraInitialized)
		{
			try
			{
				var cameraFeed = CameraDevice.Instance.GetCameraImage(Vuforia.Image.PIXEL_FORMAT.RGB888);
				if (cameraFeed == null)
				{
					return;
				}
				var data = barCodeReader.Decode(cameraFeed.Pixels, cameraFeed.BufferWidth, cameraFeed.BufferHeight, RGBLuminanceSource.BitmapFormat.RGB24);
				if (data != null)
				{
					// QRCode detected.
					OutputText.text = data.Text;
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}
		}
	}    
}
