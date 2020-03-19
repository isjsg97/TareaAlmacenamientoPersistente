using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Text;

namespace OpenTDB
{
    public class TrivialAPI : MonoBehaviour
    {
        private const int MAX_QUESTION = 50;
        private const string URL_BASE = "https://opentdb.com/";

        private class Request
        {
            public bool Error { get; set; }
            public string Result { get; set; }
            public string Url { get; set; }
        }

        public Result<Category[]> GetCategories()
        {
            var result = new Result<Category[]>();
            StartCoroutine(GetCategoriesCor(result));
            return result;
        }

        public Result<Difficulty[]> GetDifficulties()
        {
            var result = new Result<Difficulty[]>();
            result.Data = new Difficulty[]
            {
                new Difficulty(){Id = -1, Name = "Any Difficulty"},
                new Difficulty(){Id = 1, Name = "Easy"},
                new Difficulty(){Id = 1, Name = "Medium"},
                new Difficulty(){Id = 1, Name = "Hard"}
            };
            result.IsCompleted = true;
            return result;
        }

        public Result<Question[]> GetQuestions(Category category, Difficulty difficulty, int count = 10)
        {
            if (count > MAX_QUESTION) count = MAX_QUESTION;
            var result = new Result<Question[]>();
            StartCoroutine(GetQuestionsCor(result, category, difficulty, count));
            return result;
        }

        IEnumerator GetCategoriesCor(Result<Category[]> result)
        {
            var request = new Request()
            {
                Url = $"{URL_BASE}api_category.php"
            };

            yield return StartCoroutine(WebRequestCor(request));

            var json = JObject.Parse(request.Result);
            var categoriesJson = json["trivia_categories"].ToString();
            var categories = JsonConvert.DeserializeObject<List<Category>>(categoriesJson);
            categories.Insert(0, new Category() { Id = -1, Name = "Any Category" });
            result.Data = categories.ToArray();
            result.IsCompleted = true;
        }

        IEnumerator GetQuestionsCor(Result<Question[]> result, Category category, Difficulty difficulty, int count)
        {
            var urlBuilder = new StringBuilder($"{URL_BASE}api.php?amount={count}");

            if (category.Id >= 0) urlBuilder.Append($"&category={category.Id}");
            if (difficulty.Id >= 0) urlBuilder.Append($"&difficulty={difficulty.Name.ToLower()}");

            var request = new Request()
            {
                Url = urlBuilder.ToString()
            };

            yield return StartCoroutine(WebRequestCor(request));

            var json = JObject.Parse(request.Result);
            var questionsJson = JArray.Parse(json["results"].ToString());
            result.Data = questionsJson.Select(
                q => new Question()
                {
                    Category = category,
                    CorrectAnswer = new Answer() { Sentence = HttpUtility.HtmlDecode(q["correct_answer"].ToString()).Trim() },
                    Difficulty = q["difficulty"].ToObject<Question.Difficulties>(),
                    IncorrectAnswers = q["incorrect_answers"]
                        .Select(a => new Answer() { Sentence = HttpUtility.HtmlDecode(a.ToString()).Trim() })
                        .ToArray(),
                    Sentence = HttpUtility.HtmlDecode(q["question"].ToString()).Trim(),
                    Type = q["type"].ToObject<Question.Types>()
                }
            ).ToArray();

            result.IsCompleted = true;
        }

        IEnumerator WebRequestCor(Request request)
        {
            var www = UnityWebRequest.Get(request.Url);

            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
                request.Error = true;
            }
            else
            {
                request.Result = www.downloadHandler.text;
                Debug.Log(request.Result);
            }
        }
    }
}
