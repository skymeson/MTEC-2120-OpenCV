namespace OpenCvSharp.Demo
{
	using System;
	using UnityEngine;
	using System.Collections.Generic;
	using UnityEngine.UI;
	using OpenCvSharp;

	public class FaceDetectorScene : WebCamera
	{
		public TextAsset faces;
		public TextAsset eyes;
		public TextAsset shapes;

		private FaceProcessorLive<WebCamTexture> processor;

		/// <summary>
		/// Default initializer for MonoBehavior sub-classes
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			base.forceFrontalCamera = true; // we work with frontal cams here, let's force it for macOS s MacBook doesn't state frontal cam correctly

			//shapes = Resources.Load("shape_predictor_68_face_landmarks.dat") as TextAsset;
			shapes = (TextAsset)Resources.Load("shape_predictor_68_face_landmarks", typeof(TextAsset));

			Debug.Log("Awake. 1");
			// Why is shapes null???
			if (shapes == null) return; 
			byte[] shapeDat = shapes.bytes;




			if (shapeDat.Length == 0)
			{
				string errorMessage =
					"In order to have Face Landmarks working you must download special pre-trained shape predictor " +
					"available for free via DLib library website and replace a placeholder file located at " +
					"\"OpenCV+Unity/Assets/Resources/shape_predictor_68_face_landmarks.bytes\"\n\n" +
					"Without shape predictor demo will only detect face rects.";

#if UNITY_EDITOR
				// query user to download the proper shape predictor
				if (UnityEditor.EditorUtility.DisplayDialog("Shape predictor data missing", errorMessage, "Download", "OK, process with face rects only"))
					Application.OpenURL("http://dlib.net/files/shape_predictor_68_face_landmarks.dat.bz2");
#else
             UnityEngine.Debug.Log(errorMessage);
#endif
			}

			processor = new FaceProcessorLive<WebCamTexture>();
			processor.Initialize(faces.text, eyes.text, shapes.bytes);

			// data stabilizer - affects face rects, face landmarks etc.
			processor.DataStabilizer.Enabled = true;        // enable stabilizer
			processor.DataStabilizer.Threshold = 2.0;       // threshold value in pixels
			processor.DataStabilizer.SamplesCount = 2;      // how many samples do we need to compute stable data

			// performance data - some tricks to make it work faster
			processor.Performance.Downscale = 256;          // processed image is pre-scaled down to N px by long side
			processor.Performance.SkipRate = 0;             // we actually process only each Nth frame (and every frame for skipRate = 0)
			if (processor == null) Debug.LogError("processor is null!");

			Debug.Log("Awake. 2");

			//processor.
			Debug.Log("Processor is Initialized. " + processor.ToString());

		}


		protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
		{
			//Mat img = Unity.TextureToMat(input, TextureParameters);

			////Convert image to grayscale
			//Mat imgGray = new Mat();
			//Cv2.CvtColor(img, imgGray, ColorConversionCodes.BGR2GRAY);

			//// Clean up image using Gaussian Blur
			//Mat imgGrayBlur = new Mat();
			//Cv2.GaussianBlur(imgGray, imgGrayBlur, new Size(5, 5), 0);

			////Extract edges
			//Mat cannyEdges = new Mat();
			//Cv2.Canny(imgGrayBlur, cannyEdges, 10.0, 70.0);

			////Do an invert binarize the image
			//Mat mask = new Mat();
			//Cv2.Threshold(cannyEdges, mask, 70.0, 255.0, ThresholdTypes.BinaryInv);

			//// result, passing output texture as parameter allows to re-use it's buffer
			//// should output texture be null a new texture will be created
			//output = Unity.MatToTexture(mask, output);
			//return true;


			if (processor == null)
			{
				return false;
			}
            //	// detect everything we're interested in

            if (input == null)
			{
				Debug.LogError("Input is null"); 
				return false;
			}

			if (TextureParameters == null)
			{
				Debug.LogError("TextureParameters is null");
				return false;
			}

			processor.ProcessTexture(input, TextureParameters);

            // mark detected objects
            processor.MarkDetected();

            // processor.Image now holds data we'd like to visualize
            output = Unity.MatToTexture(processor.Image, output);   // if output is valid texture it's buffer will be re-used, otherwise it will be re-created

            return true;


        }


		/// <summary>
		/// Per-frame video capture processor
		/// </summary>
		//protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
		//{
		//	// detect everything we're interested in
		//	processor.ProcessTexture(input, TextureParameters);

		//	// mark detected objects
		//	processor.MarkDetected();

		//	// processor.Image now holds data we'd like to visualize
		//	output = Unity.MatToTexture(processor.Image, output);   // if output is valid texture it's buffer will be re-used, otherwise it will be re-created

		//	return true;
		//}
	}
}