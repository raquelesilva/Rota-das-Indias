using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CoreSystems.Managers
{
	public class NotificationManager : MonoBehaviour
	{
		// ### A duração total da animação quando este abre/fecha
		[SerializeField] float duration = 1f;

		// ### Tipo de função que terá um efeito especifico na animação de Easing
		[SerializeField] EaseFunctions.Ease easeFunction;

		// ### Referência do objeto da janela
		[SerializeField] Transform window = null;
		[SerializeField] Sprite winWindow = null;
		[SerializeField] Sprite loseWindow = null;
		[SerializeField] Sprite infoWindow = null;

		// ### Referência do objeto que terá mensagem
		[SerializeField] TextMeshProUGUI message = null;

		[SerializeField] Color32 colorRight = Color.white;
		[SerializeField] Color32 colorWrong = Color.white;

		public static NotificationManager instance;

        private void Awake()
        {
            instance = this;

            window.transform.localScale = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// Define a mensagem, a cor da mensagem e abre a janela da notificação
        /// </summary>
        public void SetMessage(string text, Color32 messageColor, string typeOfMessage, float duration=1f)
		{
			Transform windowType;

			if(typeOfMessage == "win")
			{
                window.GetComponent<Image>().sprite = winWindow;
            }else if (typeOfMessage == "lose")
            {
                window.GetComponent<Image>().sprite = loseWindow;
            }
            else
            {
                window.GetComponent<Image>().sprite = infoWindow;
            }

            this.duration = duration;
            window.gameObject.SetActive(false);
            StopCoroutine(nameof(StartMessage));
            window.gameObject.SetActive(true);
            window.localScale = Vector3.zero;

			message.text = text;
			message.color = messageColor;
			var img = window.gameObject.GetComponent<Image>();
			if(img == null)
			{
				Debug.Log("O componente Image está em falta no Notification Manager.");
				return;
			}

			//img.color = messageColor;

			StartCoroutine(nameof(StartMessage));
		}

		/// <summary>
		/// Retorna a duração definida
		/// </summary>
		public float GetDuration() 
		{ 
			return duration;
		}

		public Color32 GetColorRight()
		{
			return colorRight;
		}

		public Color32 GetColorWrong()
		{
			return colorWrong;
		}

		/// <summary>
		/// Contém o processo de animação da janela
		/// </summary>
		private IEnumerator StartMessage()
		{
			float currentDuration = 0f;
			float currentPercentage = 0f;

			while(currentPercentage < 1f)
			{
				currentDuration += Time.deltaTime;

				currentPercentage = currentDuration / duration;

				var scaleMode = EaseFunctions.GetEasingFunction(easeFunction);

				float value = scaleMode(0f, 1f, currentPercentage);

				window.localScale = new Vector3(value, value, value);

				yield return null;
			}

			yield return new WaitForSeconds(5);

			StartCoroutine(CloseMessage());
		}

		public void DisableWindow()
		{
			window.gameObject.SetActive(false);
		}

        private IEnumerator CloseMessage()
        {
            float currentDuration = 0f;
            float currentPercentage = 0f;

            while (currentPercentage < 1f)
            {
                currentDuration += Time.deltaTime;

                currentPercentage = currentDuration / duration;

                var scaleMode = EaseFunctions.GetEasingFunction(easeFunction);

                float value = scaleMode(1f, 0f, currentPercentage);

                window.localScale = new Vector3(value, value, value);

                yield return null;
            }


			window.transform.localScale = new Vector3(0, 0, 0);
        }
    }
}