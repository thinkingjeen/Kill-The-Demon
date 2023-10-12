using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;
using Random = System.Random;

namespace MobileConsole
{
	internal class Demo : MonoBehaviour
	{
		[SerializeField]
		private bool generateLogsWithKeys = false;

		private float _countdown = 1.0f;
		private Thread _thread;

		void Start()
		{
			Application.targetFrameRate = 60;
		}

		void Update()
		{
			if (generateLogsWithKeys)
			{
				if (Input.GetKeyDown(KeyCode.Q))
				{
					Debug.Log("Info 1");
				}

				if (Input.GetKeyDown(KeyCode.W))
				{
					Debug.LogWarning("Warning 1");
				}

				if (Input.GetKeyDown(KeyCode.E))
				{
					Debug.LogError("Error 1");
				}

				if (Input.GetKeyDown(KeyCode.A))
				{
					Debug.Log("Info 2");
				}

				if (Input.GetKeyDown(KeyCode.S))
				{
					Debug.LogWarning("Warning 2");
				}

				if (Input.GetKeyDown(KeyCode.D))
				{
					Debug.LogError("Error 2");
				}
			}
			else
			{
				_countdown -= Time.deltaTime;
				if (_countdown <= 0.0f)
				{
					int n = (int) (new Random().NextDouble() * 10.0f);
					RandomLogCall(n);
					_countdown = 2.0f;
				}
			}
		}

		private void RandomLogCall(int depth)
		{
			if (depth > 0)
			{
				RandomLogCall(depth - 1);
			}
			else
			{
				double n = new Random().NextDouble();
				if (n < 0.33)
				{
					Debug.Log("A really long info text that should wrap at some point, testing it out right now...");
				}
				else if (n < 0.66)
				{
					double n2 = new Random().NextDouble();
					if (n2 < 0.5)
					{
						Debug.LogWarning("Warning");
					}
					else
					{
						_thread = new Thread(LogCallFromThread);
						_thread.Name = "LogCallThread";
						_thread.IsBackground = true;
						_thread.Start();
					}
				}
				else
				{
					double n2 = new Random().NextDouble();
					if (n2 < 0.33)
					{
						Debug.LogError("Error");
					}
					else if (n2 < 0.66)
					{
						// Exception
						string s = null;
						s.IndexOf("null!");
					}
					else
					{
						// Assert
						Assert.IsTrue(false);
					}
				}
			}
		}

		private void LogCallFromThread()
		{
			Debug.LogWarning("Warning from thread");
		}
	}
}