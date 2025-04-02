﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public abstract class CommonHubUI : MonoBehaviour
{
    public bool IsBackButtonEnabled;
    public VisualTreeAsset HubElementUI;
    public UIDocument HubUI;
    
    protected VisualElement hubContainerRoot;
    protected VisualElement hubContainer;

    public virtual void Start()
    {
        InitUI();        
    }

    void InitUI()
    {
        hubContainerRoot = HubUI.rootVisualElement;
        hubContainer = hubContainerRoot.Q<VisualElement>("hub-container");

        InitHubElements();
        InitHubName();
        InitBackButtonVisibility();
        InitBackButtonClick();
    }

    // TODO не разобрался как это лучше сделать
    public abstract void InitHubName();

    public abstract void InitBackButtonClick ();
    public void InitBackButtonVisibility()
    {
        hubContainerRoot.Q<Button>("back-btn").visible = IsBackButtonEnabled;
    }

    public abstract void InitHubElements();

    public void InitHubElement(string name, Texture2D iconSprite, object arg)
    {
        TemplateContainer hubElement = HubElementUI.Instantiate();
        VisualElement hubRoot = hubElement.Q<VisualElement>("template");

        Button link = hubRoot.Q<Button>("link-btn");
        VisualElement icon = hubRoot.Q<VisualElement>("icon");
        Label nameLabel = hubRoot.Q<Label>("name-label");

        nameLabel.text = name;
        if (iconSprite != null)
        {
            icon.style.backgroundImage = iconSprite;
        }

        MakeLinkButton(link, arg);

        hubContainer.Add(hubRoot);
    }

    protected abstract void MakeLinkButton(Button link, object arg);

}
