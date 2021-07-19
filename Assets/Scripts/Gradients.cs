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
        GreenGreen = GenGradientThreeColors(Color.green, Color.blue, Color.green);
        GreenRed = GenGradientThreeColors(Color.green, Color.blue, Color.red);
        RedGreen = GenGradientThreeColors(Color.green, Color.blue, Color.green);
        RedRed = GenGradientThreeColors(Color.red, Color.blue, Color.red);
        GreenBlue = GenGradientTwoColors(Color.green, Color.blue);
        RedBlue = GenGradientTwoColors(Color.red, Color.blue);
    }

    public static Gradient GenGradientTwoColors(Color firstColor, Color secondColor)
    {
        var grad = new Gradient();
        var gck = new GradientColorKey[2];
        var gak = new GradientAlphaKey[2];
        gck[0].time = -1.0F;
        gck[1].time = 1.0F;
        gak[0].time = -1.0F;
        gak[1].time = 1.0F;
        gak[0].alpha = 255F;
        gak[1].alpha = 255F;
        gck[0].color = firstColor;
        gck[1].color = secondColor;
        grad.SetKeys(gck, gak);
        return grad;
    }

    public static Gradient GenGradientThreeColors(Color firstColor, Color secondColor, Color thirdColor)
    {
        var grad = new Gradient();
        var gck = new GradientColorKey[3];
        var gak = new GradientAlphaKey[3];
        gck[0].time = 0F;
        gck[1].time = 0.5F;
        gck[2].time = 1.0F;
        gak[0].time = 0F;
        gak[1].time = 0.5F;
        gak[2].time = 1.0F;
        gak[0].alpha = 1F;
        gak[1].alpha = 1F;
        gak[2].alpha = 1F;
        gck[0].color = firstColor;
        gck[1].color = secondColor;
        gck[2].color = thirdColor;
        grad.SetKeys(gck, gak);
        return grad;
    }
}