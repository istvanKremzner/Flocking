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

    public void SetOptionFriend()
    {
        if (loaded)
            controller.optionFriend = OptionFriend.isOn;
    }

    public void SetOptionCrowd()
    {
        if (loaded)
            controller.optionCrowd = OptionCrowd.isOn;
    }

    public void SetOptionAvoid()
    {
        if (loaded)
            controller.optionAvoid = OptionAvoid.isOn;
    }

    public void SetOptionNoise()
    {
        if (loaded)
            controller.optionNoise = OptionNoise.isOn;
    }

    public void SetOptionCohese()
    {
        if (loaded)
            controller.optionCohese = OptionCohese.isOn;
    }

    public void SetMaxSpeed()
    {
        if (loaded)
            controller.maxSpeed = MaxSpeed.value;
    }

    public void SetFriendRadius()
    {
        if (loaded)
            controller.friendRadius = FriendRadius.value;
    }

    public void SetCrowdRadius()
    {
        if (loaded)
            controller.crowdRadius = CrowdRadius.value;
    }
    public void SetObstacleAvoidRadius()
    {
        if (loaded)
            controller.avoidRadius = ObstacleAvoidRadius.value;
    }
    public void SetNoiseLevel()
    {
        if (loaded)
            controller.maxNoise = NoiseLevel.value;
    }

    public void SetCoheseRadius()
    {
        if (loaded)
            controller.coheseRadius = CoheseRadius.value;
    }

    public void SetInitNumber()
    {
        if (loaded)
            controller.InitNumber = int.Parse(InitNumber.text);
    }

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

    public void SwitchTime(bool on)
    {
        if (on)
            Time.timeScale = 1.0f;
        else
            Time.timeScale = 0.0f;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShutDown()
    {
        Application.Quit();
    }

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

    private void Awake()
    {
        GetValues();
    }



}
