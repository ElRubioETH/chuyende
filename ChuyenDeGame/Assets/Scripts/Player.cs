using Fusion;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class Player : NetworkBehaviour 
{
    public string Name;//ten
    public CinemachineCamera FollowCamera;// cam theo ng chs
    public TextMeshProUGUI nameText;
    public CharacterController _CharacterController ;// diu khien nhan vat
    public float Speed = 5;

    [Networked, OnChangedRender (nameof( OnChangedHealth))]
    public int health { get; set; }//mau
    public int Health { get; private set; }

    public TextMeshProUGUI healthText;//hien thi mau

    private void OnChangedHealth()
    {
        healthText.text = health.ToString();
    }

    

   
    //chay ngay sau khi nhan vat vao game
    public override void Spawned()
    {
        base.Spawned();
        FollowCamera = FindAnyObjectByType<CinemachineCamera>();
        if (Object.HasInputAuthority && FollowCamera !=null)
        {
            FollowCamera.Follow = transform;// tao cho moi nhan vat 1 camera rieng 
            FollowCamera.LookAt = transform;

        }

        if (Object.HasInputAuthority )
        {
            Name = PlayerPrefs.GetString("PlayerName");

            nameText.text = name;

            _CharacterController = GetComponent<CharacterController>();

        }
        if (Object.HasInputAuthority )
        {
            RpcUpdateHealth(30);

        }
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcUpdateHealth(int health)
    {
        Health=health;
        healthText.text = $"{Health}";
    }
        
    public override void FixedUpdateNetwork()
    {
        // xoay ten va mau cua nhan vat 

        if(FollowCamera  != null)
        {
            nameText.transform.LookAt(FollowCamera.transform);
            healthText .transform.LookAt(FollowCamera.transform);  
        }
        
        if (Object.HasInputAuthority )
        {
            var move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                _CharacterController.Move(move * Time.deltaTime * 5);

        }
    }
}
