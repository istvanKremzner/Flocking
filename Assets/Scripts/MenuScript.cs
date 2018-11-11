using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject fishPrefab;
    public GameObject birdPrefab;

    [Space]
    public Camera mainCamera;
    public BoxCollider bounds;
    [Header("Dropdowns")]
    public Dropdown CameraMode;
    public Dropdown SelectedAnimal;

    [Space]
    public InputField InitNumber;

    [Header("Options")]
    public Toggle OptionFriend;
    public Toggle OptionCrowd;
    public Toggle OptionAvoid;
    public Toggle OptionNoise;
    public Toggle OptionCohese;

    [Header("Sliders")]
    public Slider MaxSpeed;
    public Slider FriendRadius;
    public Slider CrowdRadius;
    public Slider ObstacleAvoidRadius;
    public Slider NoiseLevel;
    public Slider CoheseRadius;

    public BoidsController controller;

    private bool loaded = false;

    /// <summary>
    /// Copies the friend options from the Toggle to the BoidController, but only if the Toggle is loaded.
    /// </summary>
    public void SetOptionFriend()
    {
        if (loaded)
            controller.optionFriend = OptionFriend.isOn;
    }

    /// <summary>
    /// Copies the crowd options from the Toggle to the BoidController, but only if the Toggle is loaded.
    /// </summary>
    public void SetOptionCrowd()
    {
        if (loaded)
            controller.optionCrowd = OptionCrowd.isOn;
    }

    /// <summary>
    /// Copies the avoid options from the Toggle to the BoidController, but only if the Toggle is loaded.
    /// </summary>
    public void SetOptionAvoid()
    {
        if (loaded)
            controller.optionAvoid = OptionAvoid.isOn;
    }

    /// <summary>
    /// Copies the noise options from the Toggle to the BoidController, but only if the Toggle is loaded.
    /// </summary>
    public void SetOptionNoise()
    {
        if (loaded)
            controller.optionNoise = OptionNoise.isOn;
    }

    /// <summary>
    /// Copies the cohese options from the Toggle to the BoidController, but only if the Toggle is loaded.
    /// </summary>
    public void SetOptionCohese()
    {
        if (loaded)
            controller.optionCohese = OptionCohese.isOn;
    }

    /// <summary>
    /// Copies the max speed from the Slider to the BoidController, but only if the Toggle is loaded.
    /// </summary>
    public void SetMaxSpeed()
    {
        if (loaded)
            controller.maxSpeed = MaxSpeed.value;
    }

    /// <summary>
    /// Copies the friend radius from the Slider to the BoidController, but only if the Toggle is loaded.
    /// </summary>
    public void SetFriendRadius()
    {
        if (loaded)
            controller.friendRadius = FriendRadius.value;
    }

    /// <summary>
    /// Copies the crowd radius from the Slider to the BoidController, but only if the Toggle is loaded.
    /// </summary>
    public void SetCrowdRadius()
    {
        if (loaded)
            controller.crowdRadius = CrowdRadius.value;
    }

    /// <summary>
    /// Copies the avoid radius from the Slider to the BoidController, but only if the Toggle is loaded.
    /// </summary>
    public void SetObstacleAvoidRadius()
    {
        if (loaded)
            controller.avoidRadius = ObstacleAvoidRadius.value;
    }

    /// <summary>
    /// Copies the noise lelevel from the Slider to the BoidController, but only if the Toggle is loaded.
    /// </summary>
    public void SetNoiseLevel()
    {
        if (loaded)
            controller.maxNoise = NoiseLevel.value;
    }

    /// <summary>
    /// Copies the cohese radius from the Slider to the BoidController, but only if the Toggle is loaded.
    /// </summary>
    public void SetCoheseRadius()
    {
        if (loaded)
            controller.coheseRadius = CoheseRadius.value;
    }

    /// <summary>
    /// Copies the init number from the Slider to the BoidController, but only if the Toggle is loaded.
    /// </summary>
    public void SetInitNumber()
    {
        if (loaded)
            controller.InitNumber = int.Parse(InitNumber.text);
    }

    /// <summary>
    /// When the camera mode is changed in the menu Dropdown, applies the appropriate changes.
    /// </summary>
    public void ChangeCameraMode()
    {
        FollowTheBoid followScript = mainCamera.GetComponent<FollowTheBoid>();
        if (CameraMode.value == 0)
        {
            followScript.enabled = false;

            mainCamera.transform.position = new Vector3(bounds.transform.position.x, bounds.transform.position.y, -420);
            mainCamera.transform.eulerAngles = new Vector3(0, 0, 0);
            //mainCamera.transform.LookAt(bounds.transform.position);
        }
        else if (CameraMode.value == 1)
        {
            followScript.enabled = true;
        }
    }

    /// <summary>
    /// When the animal is changed in the menu Dropdown, changes the prefab in the BoidController.
    /// </summary>
    public void ChangeAnimal()
    {
        FollowTheBoid followScript = mainCamera.GetComponent<FollowTheBoid>();
        if (SelectedAnimal.value == 0)
        {
            //controller.prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefab/Fish.prefab", typeof(GameObject));
            controller.prefab = fishPrefab;
        }
        else if (SelectedAnimal.value == 1)
        {
            //controller.prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefab/Bird.prefab", typeof(GameObject));
            controller.prefab = birdPrefab;
        }
    }

    /// <summary>
    /// Switches between frozen time and normal time.
    /// </summary>
    /// <param name="on"></param>
    public void SwitchTime(bool on)
    {
        if (on)
            Time.timeScale = 1.0f;
        else
            Time.timeScale = 0.0f;
    }

    /// <summary>
    /// Reloads the current scene.
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void ShutDown()
    {
        Application.Quit();
    }

    /// <summary>
    /// Copies the values for the Menu items from the BoidController for initalize.
    /// </summary>
    private void GetValues()
    {
        loaded = true;

        InitNumber.text = controller.InitNumber + "";

        OptionFriend.isOn = controller.optionFriend;
        OptionCrowd.isOn = controller.optionCrowd;
        OptionAvoid.isOn = controller.optionAvoid;
        OptionNoise.isOn = controller.optionNoise;
        OptionCohese.isOn = controller.optionCohese;

        MaxSpeed.value = controller.maxSpeed;
        FriendRadius.value = controller.friendRadius;
        CrowdRadius.value = controller.crowdRadius;
        ObstacleAvoidRadius.value = controller.avoidRadius;
        NoiseLevel.value = controller.maxNoise;
        CoheseRadius.value = controller.coheseRadius;
    }

    /// <summary>
    /// Executes shortly after the scene is loaded.
    /// </summary>
    private void Awake()
    {
        GetValues();
    }



}
