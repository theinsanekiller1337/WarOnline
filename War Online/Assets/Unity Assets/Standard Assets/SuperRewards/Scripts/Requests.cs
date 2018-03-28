using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

namespace Playerize
{
	/// <summary>
	/// Web requests to fetch SuperRewards data.
	/// </summary>
	static class Requests
	{
		/// <summary>
		/// Makes a request to the given URL.
		/// </summary>
		/// <param name="url">The URL endpoint.</param>
		/// <param name="successCallback">The callback to execute on request success.</param>
		/// <param name="errorCallback">The callback to execute on request failure.</param>
		internal static Coroutine MakeRequest (string url, WWWForm postData, Action<object> successCallback, Action errorCallback=null)
		{
			return SuperRewards.RunRoutine(MakeRequestRoutine(url, postData, successCallback, errorCallback));
		}


		static IEnumerator MakeRequestRoutine (string url, WWWForm postData, Action<object> successCallback, Action errorCallback)
		{
			WWW www;

			if (postData != null) {
				www = new WWW(url, postData);
			} else {
				www = new WWW(url);
			}

			yield return www;

			if (SuperRewards.debug) { Debug.Log("[SuperRewards] Request: " + www.text); }

			if (!string.IsNullOrEmpty(www.error)) {
				Debug.LogError("[SuperRewards] Error making request: " + www.error);

				if (errorCallback != null) {
					errorCallback();
				}
			} else { // Request successful
				// Use XML parser.
				if (url.IndexOf("xml=1") != -1) {
					var parser = new XMLParser();
					var xml = new XMLObj();

					using (TextReader sr = new StringReader(www.text)) {
						parser.Parse(sr, xml);
					}

					successCallback(xml.data);
				}
				// Use JSON parser.
				else {
					var json = JSON.Parse(www.text);
					successCallback(json);
				}
			}
		}

		/// <summary>
		/// Loads an image into a Texture2D object.
		/// </summary>
		/// <param name="url">The image URL endpoint.</param>
		/// <param name="successCallback">The callback to execute on request success.</param>
		/// <param name="errorCallback">The callback to execute on request failure.</param>
		internal static Coroutine LoadImage (string url, Action<Texture2D> successCallback, Action errorCallback=null)
		{
			return SuperRewards.RunRoutine(LoadImageRoutine(url, successCallback, errorCallback));
		}

		static IEnumerator LoadImageRoutine (string url, Action<Texture2D> successCallback, Action errorCallback)
		{
			var www = new WWW(url);
			yield return www;

			if (!string.IsNullOrEmpty(www.error)) {
				Debug.LogError("[SuperRewards] Error loading image from " + url + ": " + www.error);

				if (errorCallback != null) {
					errorCallback();
				}
			} else { // Request successful
				Texture2D texture;

				// Unity cannot natively load GIF images, so use our own decoder if we receive one.
				if (IsGif(url)) {
					var gif = new GIFDecoder(www.bytes);
					texture = gif.texture;
				} else {
					texture = www.texture;
				}

				successCallback(texture);
			}
		}

		/// <summary>
		/// Determines whether a URL points to a GIF.
		/// </summary>
		/// <param name="url">The image URL endpoint.</param>
		static bool IsGif (string url)
		{
			var extension = url.Substring(url.LastIndexOf('.') + 1).ToLower();
			return extension == "gif";
		}
	}
}
