using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

string resp = "";
bool quit = false;
string fileName = "questeion.txt";
List<Questeion> questions;


while (!quit){
    Console.WriteLine(" WHat do you want to Do (G) Take away some Questions(T) Do the quiz(K) Quit(Q)");
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
            Console.WriteLine("YOU CAN DO THAT");
            break;
    }


}

void editQuestions()
{
    Console.WriteLine("");
    Console.WriteLine("Add a Questions");
    string questeion = Console.ReadLine();
    int numbOfQuestions = 0;
    bool validAnswer = false;
    while(!validAnswer){
         Console.WriteLine("");
        Console.WriteLine("How many answers exist 2-4");

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
        Console.WriteLine("");
        Console.WriteLine("What are the answer optiosn " + i);
        answerPossebiltys[i] = Console.ReadLine();
    }

    int correctAnswer = 0;
    validAnswer = false;
    while(!validAnswer){
        Console.WriteLine("Which is the right answer 0-" + (numbOfQuestions-1));

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
         Console.WriteLine("");
        Console.WriteLine("NO QUESTIONS EXISTS");
    }
    else {
        questions = JsonSerializer.Deserialize<List<Questeion>>(File.ReadAllText(fileName));
        for (int i = 0;  i < questions.Count; i++){
            Console.WriteLine(i + " " + questions[i].QuesteionString);
        }
        Console.WriteLine("");
        Console.WriteLine("Write the number you want to take away");

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
        Console.WriteLine("");
        Console.WriteLine("NO QUESTIONS EXISTS");
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
                    Console.WriteLine("THATS NOT A VALUE");
                    }
                }
                else {
                    Console.WriteLine("THATS NOT A VALUE");
                }
            }
            if (currentQuestion.checkForAnswer(guess)){
                Console.WriteLine("CORRECT");
                points++;
            }
            else {
                Console.WriteLine("WROOOONONNNGGG");
            }
        }
        Console.WriteLine("You got " + points + " of " + questions.Count);
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
        Console.WriteLine("");
        Console.WriteLine(QuesteionString);
        for (int i = 0; i < Answers.Length; i++)
        {
            Console.WriteLine("Svar " + i + ": " + Answers[i]);
        }
        Console.WriteLine("Answer with a number");
    }

    public bool checkForAnswer(int index)
    {
        return (index == CorrectAnswerIndex) ? true : false;
    }
}
