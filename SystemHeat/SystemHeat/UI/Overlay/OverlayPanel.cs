﻿using UnityEngine;
using UnityEngine.UI;
using KSP.Localization;
using System.Data;

namespace SystemHeat.UI
{
  public class OverlayPanel : MonoBehaviour
  {
    public bool active = true;
    public bool panelOpen = false;
    public RectTransform rect;
    public GameObject infoPanel;
    public GameObject icon;
    public RectTransform infoPanelTop;
    public Image colorRing;
    public Image colorRingAnimated;
    public ImageRotateAnimator colorRingAnimator;
    public Image systemIcon;
    public Image systemIconBackground;
    public Image heatIcon;
    public Image heatIconBackground;
    public Image heatIconGlow;

    public Image tempBarBackground;
    public Image tempBar;

    public Button iconButton;

    public Text infoPanelTitle;
    public Text infoPanelUpperText;
    public Text infoPanelLowerText;

    public ModuleSystemHeat heatModule;
    public Canvas parentCanvas;
    public HeatLoop loop;

    public Gradient temperatureGradient;

    public void SetupComponents()
    {
      // Find all the components
      rect = this.GetComponent<RectTransform>();
      icon = transform.FindDeepChild("Icon").gameObject;
      infoPanel = transform.FindDeepChild("InfoPanel").gameObject;

      colorRing = transform.FindDeepChild("IconColorRing").GetComponent<Image>();
      colorRingAnimated = transform.FindDeepChild("IconSpin").GetComponent<Image>();
      colorRingAnimator = colorRingAnimated.gameObject.AddComponent<ImageRotateAnimator>();
      colorRingAnimator.SpinRate = 100f;
      systemIcon = transform.FindDeepChild("IconForeground").GetComponent<Image>();
      tempBarBackground = transform.FindDeepChild("TempBarOutline").GetComponent<Image>();
      tempBar = transform.FindDeepChild("TempBar").GetComponent<Image>();

      temperatureGradient = new Gradient();
      temperatureGradient.colorKeys = new GradientColorKey[]
        {
          new GradientColorKey(new Color(0f, 1f, 0f), 0f),
          new GradientColorKey(new Color(0.98f, 0.91f, 0f), 0.33f),
          new GradientColorKey(new Color(0.98f, 0.54f, 0f), 0.66f),
          new GradientColorKey(new Color(1f, 0f, 0f), 1f)
        };

      heatIconBackground = transform.FindDeepChild("FireButton").GetComponent<Image>();
      heatIcon = transform.FindDeepChild("ThermoIcon").GetComponent<Image>();

      heatIconGlow = transform.FindDeepChild("FireGlow").GetComponent<Image>();
      heatIconGlow.gameObject.AddComponent<ImageFadeAnimator>();

      systemIconBackground = transform.FindDeepChild("InfoIconBackground").GetComponent<Image>();
      iconButton = transform.FindDeepChild("InfoIconBackground").GetComponent<Button>();
      infoPanelTop = transform.FindDeepChild("InfoPanelTop").GetComponent<RectTransform>();
      infoPanelTitle = transform.FindDeepChild("InfoPanelTitleText").GetComponent<Text>();
      infoPanelUpperText = transform.FindDeepChild("InfoPanelTopText").GetComponent<Text>();
      infoPanelLowerText = transform.FindDeepChild("InfoPanelBottomText").GetComponent<Text>();

      iconButton.onClick.AddListener(delegate { OnButtonClick(); });
      SetPanel(panelOpen);

      heatIconBackground.enabled = false;
      heatIconGlow.enabled = false;
      heatIcon.enabled = false;
     
    }

    public void OnButtonClick()
    {
      SetPanel(!panelOpen);
    }

    public void LateUpdate()
    {
      if (active && loop != null && heatModule != null)
      {
        //if (panelOpen)
        //{
        //  if (heatModule.totalSystemFlux <= 0f)
        //  {
        //    infoPanelUpperText.text = Localizer.Format("#LOC_SystemHeat_OverlayPanel_UpperTextNoTemp", Utils.ToSI(heatModule.totalSystemFlux, "F0"));
        //    infoPanelTop.sizeDelta = new Vector2(infoPanelTop.sizeDelta.x, 26f);
        //  }
        //  else
        //  {
        //    infoPanelUpperText.text = Localizer.Format("#LOC_SystemHeat_OverlayPanel_UpperText", heatModule.systemNominalTemperature.ToString("F0"), Utils.ToSI(heatModule.totalSystemFlux, "F0"));
        //    infoPanelTop.sizeDelta = new Vector2(infoPanelTop.sizeDelta.x, 48f);
        //  }

        //  infoPanelLowerText.text = Localizer.Format("#LOC_SystemHeat_OverlayPanel_LowerText", loop.Temperature.ToString("F0"), loop.NominalTemperature.ToString("F0"), Utils.ToSI(loop.NetFlux, "F0"), loop.Volume.ToString("F2"));
        //}

        //float nominalTempDelta = loop.NominalTemperature - heatModule.systemNominalTemperature;
        //float tempDelta = loop.Temperature - heatModule.systemNominalTemperature;
        float fracPerDegree = 0.331f / 2000f;
        tempBarBackground.fillAmount = Mathf.Min(heatModule.systemNominalTemperature * fracPerDegree + 0.015f, 0.331f);
        tempBar.fillAmount = Mathf.Min(loop.Temperature * fracPerDegree, 0.331f);
        float nominalTempDelta = (loop.Temperature - heatModule.systemNominalTemperature)/(2000f-heatModule.systemNominalTemperature);

        if (heatModule.systemNominalTemperature <= 0f)
        {
          if (tempBarBackground.enabled)
          {
            tempBarBackground.enabled = false;
            tempBar.enabled = false;
          }
          //if (heatIconBackground.enabled)
          //{
          //  heatIconBackground.enabled = false;
          //  heatIconGlow.enabled = false;
          //  heatIcon.enabled = false;
          //}
        }
        else
        {
          if (!tempBarBackground.enabled)
          {
            tempBarBackground.enabled = true;
            tempBar.enabled = true;
          }
          tempBar.color = temperatureGradient.Evaluate(nominalTempDelta);
          //if ((nominalTempDelta > 10f || tempDelta > 10f) && !heatIconBackground.enabled)
          //{
          //  heatIconBackground.enabled = true;
          //  heatIconGlow.enabled = true;
          //  heatIcon.enabled = true;
          //}

          //if ((nominalTempDelta <= 10f && tempDelta <= 10f) && heatIconBackground.enabled)
          //{
          //  heatIconBackground.enabled = false;
          //  heatIconGlow.enabled = false;
          //  heatIcon.enabled = false;
          //}
        }
        
          colorRingAnimator.Animate = Mathf.Abs(heatModule.totalSystemFlux) > 0.1f;
        
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(heatModule.part.transform.position);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.GetComponent<RectTransform>(), screenPoint, parentCanvas.worldCamera, out localPoint);
        transform.localPosition = localPoint;
      }
      if (heatModule == null)
      {
        Destroy(this.gameObject);
      }
    }

    public Vector3 worldToUISpace(Canvas parentCanvas, Vector3 worldPos)
    {
      //Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
      Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
      Vector2 movePos;

      //Convert the screenpoint to ui rectangle local point
      RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);
      //Convert the local point to world point
      return parentCanvas.transform.TransformPoint(movePos);
    }

    public void SetupLoop(HeatLoop lp, ModuleSystemHeat sh, bool visible)
    {
      SetupComponents();
      loop = lp;
      heatModule = sh;
      // Utils.Log($"{loop} {heatModule}, {colorRing}, {infoPanelTitle}");
      infoPanelTitle.text = heatModule.part.partInfo.title;
      colorRing.color = SystemHeatSettings.GetLoopColor(loop.ID);
      Transform xform = transform.FindDeepChild(heatModule.iconName);
      if (xform != null)
      {
        systemIcon.sprite = xform.GetComponent<Image>().sprite;
      }
      SetVisibility(visible);
    }

    public void UpdateLoop(HeatLoop lp, ModuleSystemHeat sh, bool visible)
    {
      loop = lp;
      heatModule = sh;
      Transform xform;
      if (heatModule != null && heatModule.part != null)
      {
        infoPanelTitle.text = heatModule.part.partInfo.title;
        xform = transform.FindDeepChild(heatModule.iconName);

        if (xform != null)
        {
          systemIcon.sprite = xform.GetComponent<Image>().sprite;
        }
      }

      colorRing.color = SystemHeatSettings.GetLoopColor(loop.ID);

      SetVisibility(visible);
    }

    public void SetPanel(bool state)
    {
      infoPanel.SetActive(state);
      panelOpen = state;
    }

    public void SetVisibility(bool state)
    {
      active = state;
      icon.SetActive(state);

      if (!state)
        SetPanel(state);


    }


  }
}
