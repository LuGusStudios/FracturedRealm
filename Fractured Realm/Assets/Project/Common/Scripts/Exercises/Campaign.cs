﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Campaign 
{
	public int nr = -1;

	public string name = "";
	public List<CampaignPart> parts = new List<CampaignPart>();

	public CampaignPart currentCampaignPart = null;
}
