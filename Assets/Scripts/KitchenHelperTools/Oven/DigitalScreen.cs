using UnityEngine;

public class DigitalScreen : MonoBehaviour {
    protected string zero = "1111110";
    protected string one = "0000110";
    protected string two = "1011011";
    protected string three = "1001111";
    protected string four = "0100111";
    protected string five = "1101101";
    protected string six = "1111101";
    protected string seven = "1000110";
    protected string eight = "1111111";
    protected string nine = "1101111";
    protected Renderer[][] digitRefs;
    protected string[] digitCodes;
    protected int val;

    public Material blankMat;
    public Material activeMat;

    public Renderer[] digit_1;
    public Renderer[] digit_2;
    public Renderer[] digit_3;

    void Start()
    {
        digitCodes = new string[] { zero, one, two, three, four, five, six, seven, eight, nine };
        digitRefs = new Renderer[][] { digit_1, digit_2, digit_3 };
    }

    protected void SetDigit(int digitIndex, int num)
    {
        Renderer[] digitRef = digitRefs[digitIndex];

        string digitCode = digitCodes[num];

        int codeSeq = 0;
        foreach (char c in digitCode)
        {
            if (c == '1')
            {
                digitRef[codeSeq++].material = activeMat;
            }

            else
            {
                digitRef[codeSeq++].material = blankMat;
            }
        }
    }
}
