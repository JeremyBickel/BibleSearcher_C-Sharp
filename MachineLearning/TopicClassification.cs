using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;

namespace MachineLearning
{
    internal class TopicClassification
    {
        //  Classify verses or sections of the text into various topics or themes.
        //
        //Techniques: Naive Bayes, Support Vector Machines, Decision Trees or ensemble methods like Random Forests.
        //Features: BookName, ChapterNumber, VerseNumber, Pericope1-4.
        //
        //  Steps for Building the Model:
        //
        //Data Preprocessing: Clean the data and handle missing values if any.
        //Feature Engineering: Extract and select the most important features for your task.
        //Text Vectorization: If you're using text data, you may need to convert it into numerical form using techniques like TF-IDF or Word Embeddings.
        //Train-Test Split: Divide your data into training and testing sets.
        //Model Training: Train the model using the training data.
        //Model Evaluation: Use metrics like accuracy, F1-score, or AUC-ROC to evaluate the model's performance on the test set.
        //Hyperparameter Tuning: Fine-tune the model to improve its performance.
        //Deployment: Once the model is trained and evaluated, you can use it for making predictions on new data.


    }

    public class NaiveBayes
    {
        // Naive Bayes is a simple probabilistic algorithm that uses Bayes’ theorem to classify the data.
        // Naive Bayes assumes that the features are independent of each other, and hence the name naive.
        // Works well for small datasets..

        // 1) Split into training and testing sets


        // 2) Use the Gaussian Naive Bayes algorithm
        // 3) Predict the labels for the testing data
        // 4) Calculate the accuracy of the model.

    }

    public class RandomForest
    {
        // Random Forest is an ensemble learning algorithm that combines multiple decision trees to classify the data.
        // Random Forest works well for large datasets and is robust to noise and outliers.

        // 1) Split into training and testing sets
        // 2) Use the Random Forest algorithm
        // 3) Predict the labels for the testing data
        // 4) Calculate the accuracy of the model

    }

    public class SVM
    {
        // Support Vector Machine(SVM) is a powerful algorithm that separates
        //     the data into different classes by finding the hyperplane that maximizes the margin between the classes.
        // SVM works well for both linearly and non-linearly separable data.

        // 1) Split into training and testing sets
        // 2) Use the Random Forest algorithm
        // 3) Predict the labels for the testing data
        // 4) Calculate the accuracy of the model

    }
}
