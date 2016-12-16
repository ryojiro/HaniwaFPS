using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkBehaviour {

	[SyncVar]
	private bool _isDead = false;
	public bool isDead {
		get { return _isDead; }
		protected set { _isDead = value; }
	}

	[SerializeField]
	private int maxHealth = 3;

	[SerializeField]
	private GameObject[] hp;

	[SyncVar]
	private int currentHealth;

	[SerializeField]
	private Behaviour[] disableOnDeath;
	private bool[] wasEnabled;

	[SerializeField]
	private AudioSource bgm;
	
	[SerializeField]
	private GameObject haniwaView;

	[SerializeField]
	private GameObject[] damagedEffect;

	[SerializeField]
	private GameObject damageFillter;

	public void Setup() {
		wasEnabled = new bool[disableOnDeath.Length];
		for (int i = 0; i < wasEnabled.Length; i++) {
			wasEnabled [i] = disableOnDeath [i].enabled;
		}
		SetDefaults ();
	}

	[ClientRpc]
	public void RpcTakeDamage(int _amount) {

		if (isDead) return;

		currentHealth -= _amount;

		hp [currentHealth].SetActive(false);
		if (currentHealth == 2)
			damagedEffect [2].SetActive (true);
		else if (currentHealth == 1)
			damagedEffect [1].SetActive (true);
		damageFilter();

		Debug.Log (transform.name + " now has " + currentHealth + " health.");

		if (currentHealth <= 0)
			Die ();
	}

	private void Die() {

		bgm.Stop ();
		haniwaView.SetActive (false);
		damagedEffect [0].SetActive (true);

		isDead = true;

		for (int i = 0; i < disableOnDeath.Length; i++) {
			disableOnDeath [i].enabled = false;
		}
		Collider _col = GetComponent<Collider>();
		if(_col != null)
			_col.enabled = false;

		Debug.Log (transform.name + " is DEAD!");

		StartCoroutine (Respawn());
	}

	private IEnumerator Respawn() {
		yield return new WaitForSeconds (GameManager.instance.matchSettings.respawnTime);
		SetDefaults ();
		Transform _spawnPoint = NetworkManager.singleton.GetStartPosition ();
		transform.position = _spawnPoint.position;

		Debug.Log (transform.name + " respawned.");
	}

	public void SetDefaults() {
		haniwaView.SetActive (true);
		bgm.Play ();

		isDead = false;
		currentHealth = maxHealth;
		for (int i = 0; i < disableOnDeath.Length; i++) {
			disableOnDeath [i].enabled = wasEnabled [i];
		}

		for (int i = 0; i < 3; i++) {
			hp [i].SetActive(true);
			damagedEffect [i].SetActive (false);
		}
		damagedEffect [0].SetActive (false);

		Collider _col = GetComponent<Collider>();
		if(_col != null)
			_col.enabled = true;
	}

	/// <summary>ダメージフィルタ制御</summary>
	public void damageFilter()
	{
		damageFillter.SetActive (true);
	}
}