using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonColorer : MonoBehaviour
{
	public Color NormalColor;
	public Color ClickColor;
	public Color HoverColor;

	void SetColor(Color color)
	{
		var p = GetComponent<MeshRenderer>();
		MaterialPropertyBlock mpb = new MaterialPropertyBlock();
		p.GetPropertyBlock(mpb);
		mpb.SetColor("_BaseColor", color);
		p.SetPropertyBlock(mpb);
	}

	public void SetColor_Hover()
	{
		SetColor(HoverColor);
	}

	public void SetColor_Click()
	{
		SetColor(ClickColor);
	}

	public void SetColor_Normal()
	{
		SetColor(NormalColor);
	}

}
