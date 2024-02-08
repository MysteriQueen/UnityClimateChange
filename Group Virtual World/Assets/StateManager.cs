using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


#if UNITY_EDITOR
using UnityEditor;
using System.Net;
#endif

public class StateManager : MonoBehaviour {

    public Canvas menuCanvas;

    private float startingOxygen;

    public Text oxygenOptimalPercentageText;
    public Text forestPopulationText;
    public Text forestLandAreaText;
    public Text treeGrowthRateText;
    public Text seaLevelRiseText;
    public Text[] animalGroupPopulationText;

    public struct Statistics {
        public float oxygenOptimalPercentage;
        public int forestPopulation;
        public float forestLandArea;
        public float treeGrowthRate; // How capable trees are able to grow (new trees be spawned). Dependant on the atmosperhic composition.
        public float seaLevelRise;
        public int[] animalGroupPopulation;
        public int animalCount;

    }

    public bool Paused { get; set; } = false;

    public float Sensitivity { get; private set; } = 1.0f;
    public float HandSensitivity { get; private set; } = 1.0f;

    public static Statistics statistics;

    public AnimalGroupManager[] animalGroups;
    private List<float> pastSeaLevelRises;

    private void Awake() {
        statistics = new Statistics();
        statistics.animalGroupPopulation = new int[5];

        pastSeaLevelRises = new List<float>();
        
    }

    private void Start() {
        startingOxygen = ForestManager.TreeCount;

        menuCanvas.enabled = false;

    }

    private void Update() {
        // Toggle menu if paused
        menuCanvas.enabled = Paused;

        // Acquire information for statistics
        statistics.oxygenOptimalPercentage = ForestManager.TreeCount / (startingOxygen - (statistics.animalCount + 1) * 3) ; // Number of trees in proportion to number of animals, maybe amount of land?
        statistics.forestPopulation = ForestManager.TreeCount;
        statistics.forestLandArea = 3.0f;
        statistics.treeGrowthRate = 4.0f;
        statistics.seaLevelRise = SeaLevelManager.GetSeaLevelRise() * 1000.0f;

        oxygenOptimalPercentageText.text = statistics.oxygenOptimalPercentage.ToString();
        forestPopulationText.text = statistics.forestPopulation.ToString();
        forestLandAreaText.text = statistics.forestLandArea.ToString();
        treeGrowthRateText.text = statistics.treeGrowthRate.ToString();
        seaLevelRiseText.text = statistics.seaLevelRise.ToString("F1");

        int count = 0;
        for (int i = 0; i < 5; i++) {
            statistics.animalGroupPopulation[i] = animalGroups[i].GetAnimalCount();
            animalGroupPopulationText[i].text = statistics.animalGroupPopulation[i].ToString();

            count += statistics.animalGroupPopulation[i];
        }

        statistics.animalCount = count;
    
    }

    private void OnApplicationFocus(bool hasFocus) {
        // Enable pause menu when application loses focus
        Paused = !hasFocus;
    }

    public void changeSensitivty(string str) {
        Sensitivity = float.Parse(str) * 0.1f;
    }

    public void changeHandSensitivty(string str) {
        HandSensitivity = float.Parse(str) * 0.1f;

    }

}

