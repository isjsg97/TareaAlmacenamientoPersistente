using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenTDB
{
    public class Question
    {
        public enum Difficulties { Easy, Medium, Hard };
        public enum Types { Boolean, Multiple };

        public Category Category { get; set; }
        public Answer CorrectAnswer { get; set; }
        public Difficulties Difficulty { get; set; }
        public Answer[] IncorrectAnswers { get; set; }
        public string Sentence { get; set; }
        public Types Type { get; set; }
    }
}
