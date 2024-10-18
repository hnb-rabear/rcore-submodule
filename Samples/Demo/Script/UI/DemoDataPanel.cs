using UnityEngine;
using RCore.Framework.UI;
using RCore.Components;
using TMPro;

namespace RCore.Demo
{
    public class DemoDataPanel : PanelController
    {
        [SerializeField] private CustomToggleSlider mTogSlider;
        [SerializeField] private CustomToggleTab mTab1;
        [SerializeField] private CustomToggleTab mTab2;
        [SerializeField] private CustomToggleTab mTab3;
        [SerializeField] private CustomToggleTab mTab4;
        [SerializeField] private JustButton mBtnSave;
        [SerializeField] private SimpleTMPButton mBtnLoad;
        [SerializeField] private ProgressBar mProgressBar;
        [SerializeField] private TMP_InputField mInputFiled;
        
        private float mTime;

        private ExampleGameKeyValueDB GameKeyValueDB => ExampleGameKeyValueDB.Instance;

        private void Start()
        {
            mProgressBar.Max = 20;
            mBtnSave.onClick.AddListener(SaveData);
            mBtnLoad.onClick.AddListener(LoadData);
        }

        private void OnEnable()
        {
            LoadData();
        }

        private void Update()
        {
            mTime += Time.deltaTime;
            mProgressBar.Value = mTime % 30f;
            //Or
            //mProgressBar.FillAmount = (mTime % 30f) / 30f;
        }

        [InspectorButton]
        private void LoadData()
        {
            mTogSlider.isOn = GameKeyValueDB.demoGroup.toggleIsOn.Value;
            mInputFiled.text = GameKeyValueDB.demoGroup.inputFieldText.Value;
            mProgressBar.Value = GameKeyValueDB.demoGroup.progressBarValue.Value;
            mTime = mProgressBar.Value;
            switch (GameKeyValueDB.demoGroup.tabIndex.Value)
            {
                case 1: mTab1.isOn = true; break;
                case 2: mTab2.isOn = true; break;
                case 3: mTab3.isOn = true; break;
                case 4: mTab4.isOn = true; break;
            }
        }

        [InspectorButton]
        private void SaveData()
        {
            GameKeyValueDB.demoGroup.toggleIsOn.Value = mTogSlider.isOn;
            GameKeyValueDB.demoGroup.inputFieldText.Value = mInputFiled.text;
            GameKeyValueDB.demoGroup.progressBarValue.Value = mProgressBar.Value;
            if (mTab1.isOn)
                GameKeyValueDB.demoGroup.tabIndex.Value = 1;
            else if (mTab2.isOn)
                GameKeyValueDB.demoGroup.tabIndex.Value = 2;
            else if (mTab3.isOn)
                GameKeyValueDB.demoGroup.tabIndex.Value = 3;
            else if (mTab4.isOn)
                GameKeyValueDB.demoGroup.tabIndex.Value = 4;
        }
    }
}