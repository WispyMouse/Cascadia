using System.Text;
using TMPro;
using UnityEngine;

public class CascadeText : MonoBehaviour
{
    public TMP_Text Text;

    public string TextString = "CASCADE";
    public float HeightDiffAtMaximum = 20f;
    public float SecondsPerCycle = 1f;
    public float AdditionalSpacingPerIndex = .1f;

    float curTime { get; set; } = 0;

    private void Update()
    {
        if (this.SecondsPerCycle < 0)
        {
            return;
        }

        this.curTime += Time.deltaTime;

        StringBuilder cascadeText = new();
        float halfHeight = this.HeightDiffAtMaximum / 2f;

        for (int ii = 0; ii < this.TextString.Length; ii++)
        {
            if (this.TextString[ii] == '\\' && this.TextString[ii + 1] == 'n')
            {
                cascadeText.Append("\n");
                ii++;
                continue;
            }

            float sampledTime = Mathf.PingPong(this.curTime + (this.AdditionalSpacingPerIndex * ii), this.SecondsPerCycle / 2f);
            float sampledHeight = Mathf.Lerp(-halfHeight, halfHeight, Mathf.InverseLerp(0, this.SecondsPerCycle, sampledTime));

            cascadeText.Append($"<voffset={sampledHeight}px>" + this.TextString[ii] + "</voffset>");
        }

        this.Text.text = cascadeText.ToString();
    }

    public void DisableAnimation()
    {
        this.SecondsPerCycle = -1;
        this.Text.text = this.TextString;
    }

    public void EnableAnimation(float timePerCycle) => this.SecondsPerCycle = timePerCycle;
}
