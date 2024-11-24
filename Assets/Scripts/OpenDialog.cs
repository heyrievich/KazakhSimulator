using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenDialog : MonoBehaviour
{
    private int talkedWithMom;

   

    public void DialogWithMom()
    {
        talkedWithMom = PlayerPrefs.GetInt("TalkedWithMom", 0);

        if (talkedWithMom == 0)
        {
            SceneManager.LoadScene("FirstDialogWithMom");
            PlayerPrefs.SetInt("TalkedWithMom", 1);
        }
        else if (talkedWithMom == 1)
        {
            SceneManager.LoadScene("SecondDialogWithMom");
            PlayerPrefs.SetInt("TalkedWithMom", 2);
        }
        Debug.Log("DialogWithMom была вызвана");
    }
}
