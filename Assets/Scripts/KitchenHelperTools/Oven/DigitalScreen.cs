using UnityEngine;

public class DigitalScreen : MonoBehaviour {
    private string zero = "1111110";
    private string one = "0000110";
    private string two = "1011011";
    private string three = "1001111";
    private string four = "0100111";
    private string five = "1101101";
    private string six = "1111101";
    private string seven = "1000110";
    private string eight = "1111111";
    private string nine = "1101111";
    private Renderer[][] digitRefs;
    private string[] digitCodes;
    protected int val = 0;

    public Material blankMat;
    public Material activeMat;

    public Renderer[] digit_1;
    public Renderer[] digit_2;
    public Renderer[] digit_3;

    protected virtual void Start()
    {
        digitCodes = new string[] { zero, one, two, three, four, five, six, seven, eight, nine };
        digitRefs = new Renderer[][] { digit_1, digit_2, digit_3 };
    }

    public int GetValue()
    {
        return val;
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
