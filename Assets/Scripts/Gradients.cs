using UnityEngine;

public class Gradients
{
    public static Gradient GreenGreen;
    public static Gradient GreenRed;
    public static Gradient RedGreen;
    public static Gradient RedRed;
    public static Gradient GreenBlue;
    public static Gradient RedBlue;

    public static void GenGradients()
    {
        GreenGreen = new Gradient();
        GreenRed = new Gradient();
        RedGreen = new Gradient();
        RedRed = new Gradient();
        GreenBlue = new Gradient();
        RedBlue = new Gradient();
        var gck = new GradientColorKey[2];
        var gak = new GradientAlphaKey[2];
        gck[0].time = -1.0F;
        gck[1].time = 1.0F;
        gak[0].time = -1.0F;
        gak[1].time = 1.0F;
        gak[0].alpha = 255F;
        gak[1].alpha = 255F;

        gck[0].color = Color.green;
        gck[1].color = Color.green;
        GreenGreen.SetKeys(gck, gak);
        gck[1].color = Color.red;
        GreenRed.SetKeys(gck, gak);
        gck[1].color = Color.blue;
        GreenBlue.SetKeys(gck, gak);
        gck[0].color = Color.red;
        RedBlue.SetKeys(gck, gak);
        gck[1].color = Color.red;
        RedRed.SetKeys(gck, gak);
        gck[1].color = Color.green;
        RedGreen.SetKeys(gck, gak);



    }

}
