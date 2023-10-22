using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using DataManagement.Managers;
using DataManagement.Serializers.ValidationExtension;

public class DataManager : BaseManager
{
    [Header("Validation File")]
    [SerializeField] private GameVersion versionFile;

    [Header("Managers")]
    [Tooltip("List of managers that must restart their content when version changes")]
    [SerializeField] private List<BaseManager> onStartExecuteList;
    [Tooltip("List of managers that must save their content when Save method is invoked")]
    [SerializeField] private List<BaseManager> onSaveExecuteList;
    [Tooltip("List of managers that must reset their content when Reset method is invoked")]
    [SerializeField] private List<BaseManager> onResetExecuteList;

    [Header("Reset")]
    [SerializeField] bool resetOnStart;

    protected new VariableSerializer Serializer
    {
        get { return (VariableSerializer)base.Serializer; }
        set { base.Serializer = value; }
    }

    protected sealed override void Awake()
    {
        Serializer = new VariableSerializer("Version", ValidatedValue());
        Serializer.Initialize();
    }

    private void Start()
    {
        if (resetOnStart) ResetData();

        if (Serializer.Get() != ValidatedValue())
        {
            SaveReset();
            foreach (BaseManager serializer in onStartExecuteList)
            {
                serializer.SaveReset();
            }
        }
    }

    protected string ValidatedValue()
    {
        return versionFile.version.Trim();
    }

    public void SaveManagers()
    {
        foreach (BaseManager serializer in onSaveExecuteList)
        {
            serializer.SaveData();
        }
    }

    public void ResetManagers()
    {
        foreach (BaseManager serializer in onResetExecuteList)
        {
            serializer.SaveReset();
        }
    }
}