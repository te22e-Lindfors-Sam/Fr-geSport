using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

string resp = "";
bool quit = false;
Questeion[] questeions = new Questeion[4];
string fileName = "questeion.txt";
List<Questeion> questions;
// string[] arrayAnswers = new string[4] { "JA", "NEJ", "KANSKE", "HAN KAMPAR NYNÄSHAMN" };
// Questeion tempQuestion = new Questeion(0, "CAMPAR RATTAN NEO?", arrayAnswers, 0);




while (!quit){
    Console.WriteLine("Vad vill du göra? Göra mer frågor? (G) Ta Bort Frågor(T) Kör Quized?(K) Sluta(Q)");
    resp = Console.ReadLine();
    switch (resp.ToUpper()){
        case "G":
            editQuestions();
            break;
        case "T":
            deleteQuestions();
            break;
        case "K":
            run();
            break;
        case "Q":
            return;
        default :
            Console.WriteLine("Haha De kan man inte göra TÖNT");
            break;
    }


}

void editQuestions()
{
    Console.WriteLine("Lägg till en ny fråga, Först vad är frågan");
    string questeion = Console.ReadLine();
    int numbOfQuestions = 0;
    bool validAnswer = false;
    while(!validAnswer){
        Console.WriteLine("Hur många svar finns det? 2-4");

        string numbOfAnswersString = Console.ReadLine();

        int value;
        if (int.TryParse(numbOfAnswersString, out value))
        {
            if (value >= 2 && value <= 4){
                numbOfQuestions = value;
                validAnswer = true;
            }
            else{
                Console.WriteLine("Not a Valid Number");
            }
        }
        else{
            Console.WriteLine("Not a Valid Number");
        }
    }

    string[] answerPossebiltys = new string[numbOfQuestions];
    for (int i = 0;i < numbOfQuestions; i++){
        Console.WriteLine("Vad är svarsallternativ " + i);
        answerPossebiltys[i] = Console.ReadLine();
    }

    int correctAnswer = 0;
    validAnswer = false;
    while(!validAnswer){
        Console.WriteLine("Vilket är rätt svar 0-" + (numbOfQuestions-1));

        string correctAnswerIndex = Console.ReadLine();

        int value;
        if (int.TryParse(correctAnswerIndex, out value))
        {
            if (value >= 0 && value < numbOfQuestions){
                correctAnswer = value;
                validAnswer = true;
            }
            else{
                Console.WriteLine("Not a Valid Number");
            }
        }
        else{
            Console.WriteLine("Not a Valid Number");
        }
    }

    writeData(new Questeion(questeion, answerPossebiltys, correctAnswer));

}

void writeData(Questeion questeion){
    if (File.ReadAllText(fileName).Length == 0){
        questions = new List<Questeion>();
    }
    else {
        questions = JsonSerializer.Deserialize<List<Questeion>>(File.ReadAllText(fileName));
    }
    questions.Add(questeion);
    try
    {
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            var opt = new JsonSerializerOptions(){ WriteIndented=true };
		    string strJson = JsonSerializer.Serialize<List<Questeion>>(questions, opt);
            writer.Write(strJson);
        }
    }
    catch (Exception exp)
    {
        Console.Write(exp.Message);
    }
}

void deleteQuestions(){
    if (File.ReadAllText(fileName).Length == 0){
        Console.WriteLine("DET FINNS INGA FRÅGOR");
    }
    else {
        questions = JsonSerializer.Deserialize<List<Questeion>>(File.ReadAllText(fileName));
        for (int i = 0;  i < questions.Count; i++){
            Console.WriteLine(i + " " + questions[i].QuesteionString);
        }
        Console.WriteLine("");
        Console.WriteLine("Skriv det nummer du vill ta bort");

        int value = 0;
        bool validAnswer = false;
        string valueString = Console.ReadLine();

        if (int.TryParse(valueString, out value))
        {
            if (0 <= value && value < questions.Count){
                questions.Remove(questions[value]);
                    try
                    {
                         using (StreamWriter writer = new StreamWriter(fileName))
                        {
                            var opt = new JsonSerializerOptions(){ WriteIndented=true };
		                     string strJson = JsonSerializer.Serialize<List<Questeion>>(questions, opt);
                             writer.Write(strJson);
                        }
                    }
                    catch (Exception exp)
                    {
                        Console.Write(exp.Message);
                    }  
            }
            else{
                Console.WriteLine("Not a Valid Number");
            }   
        }
        else{
            Console.WriteLine("Not a Valid Number");
        }
        

    }
}

void run(){
    int points = 0;
    if (File.ReadAllText(fileName).Length == 0){
        Console.WriteLine("DET FINNS INGA FRÅGOR");
    }
    else {
        questions = JsonSerializer.Deserialize<List<Questeion>>(File.ReadAllText(fileName));
        for (int i = 0;  i < questions.Count; i++){
            Questeion currentQuestion = questions[i];
            currentQuestion.askQuestion();
            bool validAnswer = false;
            int guess = 0;
            while (!validAnswer){
                string guessString = Console.ReadLine();
                int value = 0;
                if (int.TryParse(guessString, out value)){
                    if (value >= 0 && value < currentQuestion.Answers.Length){
                        guess = value;
                        validAnswer = true;
                    }
                    else {
                    Console.WriteLine("Det värder är inte korrkt");
                    }
                }
                else {
                    Console.WriteLine("Det värder är inte korrkt");
                }
            }
            if (currentQuestion.checkForAnswer(guess)){
                Console.WriteLine("DU HADE RÄTT");
                points++;
            }
            else {
                Console.WriteLine("DU HADE FEL");
            }
        }
        Console.WriteLine("Du hade " + points + " av " + questions.Count);
        Thread.Sleep(2000);
    }
}

// [Serializable]
public class Questeion
{
    public string QuesteionString { get; set; }
    public string[] Answers { get; set; }
    public int CorrectAnswerIndex { get; set; }

    public Questeion(string questeionString, string[] answers, int correctAnswerIndex)
    {
        this.QuesteionString = questeionString;
        this.Answers = answers;
        this.CorrectAnswerIndex = correctAnswerIndex;
    }

    public void askQuestion()
    {
        Console.WriteLine(QuesteionString);
        for (int i = 0; i < Answers.Length; i++)
        {
            Console.WriteLine("Svar " + i + ": " + Answers[i]);
        }
        Console.WriteLine("Svara nu med ett av nummrerna från frågorna");
    }

    public bool checkForAnswer(int index)
    {
        return (index == CorrectAnswerIndex) ? true : false;
    }
}
