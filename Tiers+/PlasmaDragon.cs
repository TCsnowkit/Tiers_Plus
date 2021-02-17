using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiersPlus
{
	using System;
	using System.Collections;
	using PreviewLabs;
	using UnityEngine;

	// Token: 0x0200000E RID: 14
	public class PlasmaDragonScript : MonoBehaviour
	{
		// Token: 0x0600005D RID: 93 RVA: 0x000046FC File Offset: 0x000028FC
		public void Awake()
		{
			this.hp = 2600000 + GameScript.challengeLevel * 8000;
			this.speed = 26f;
			this.exp = 9999 + GameScript.challengeLevel * 500;
			if (Network.isServer)
			{
				base.StartCoroutine(this.Shoot());
			}
			this.r = base.GetComponent<Rigidbody>();
			this.t = base.transform;
			if (this.parent)
			{
				base.StartCoroutine(this.UpdateTarget());
			}
			else if (Network.isServer)
			{
				base.StartCoroutine(this.UpdateTarget());
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00004954 File Offset: 0x00002B54
		public IEnumerator Shoot()
		{
			for (; ; )
			{
				yield return new WaitForSeconds(0.5f);
				if (UnityEngine.Random.Range(0, 5) == 0)
				{
					this.r.velocity = new Vector3(0f, 0f, 0f);
					Vector3 targetPos = new Vector3(base.transform.position.x + (float)UnityEngine.Random.Range(-100, 101), this.t.position.y + (float)UnityEngine.Random.Range(-100, 101), 0f);
					yield return new WaitForSeconds(0.5f);
					base.GetComponent<NetworkView>().RPC("Au", RPCMode.All, new object[0]);
					if (UnityEngine.Random.Range(0, 4) == 0)
					{
						GameObject gameObject = (GameObject)Network.Instantiate(this.mykOrb, this.t.position, Quaternion.identity, 0);
						gameObject.SendMessage("EnemySet", targetPos, SendMessageOptions.DontRequireReceiver);
					}
					else
					{
						GameObject gameObject2 = (GameObject)Network.Instantiate(this.mykMeteor, this.t.position, Quaternion.identity, 0);
						gameObject2.SendMessage("EnemySet", targetPos, SendMessageOptions.DontRequireReceiver);
					}
					
				}
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x0000317C File Offset: 0x0000137C
		[RPC]
		public void Au()
		{
			base.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/des"), Menuu.soundLevel / 10f);
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00004970 File Offset: 0x00002B70
		public IEnumerator UpdateTarget()
		{
			float w = 1f;
			if (this.parent)
			{
				if (this.isGlaedria)
				{
					w = 0.05f;
				}
				else
				{
					w = 0.1f;
				}
			}
			else
			{
				w = 1.5f;
			}
			for (; ; )
			{
				this.pos = Camera.main.WorldToScreenPoint(base.transform.position);
				if (this.parent)
				{
					this.dir = Camera.main.WorldToScreenPoint(this.parent.transform.position) - this.pos;
				}
				else if (this.target)
				{
					this.dir = Camera.main.WorldToScreenPoint(this.target.position) - this.pos;
				}
				else
				{
					this.dir = this.pos;
				}
				this.angle = Mathf.Atan2(this.dir.y, this.dir.x) * 57.29578f;
				this.dood = Quaternion.AngleAxis(this.angle, Vector3.forward);
				this.turnValue = (float)UnityEngine.Random.Range(2, 6);
				if (this.parent && this.isGlaedria)
				{
					this.turnValue = 50f;
				}
				if (this.isMykonogre || this.isGlaedria)
				{
					this.turnValue = (float)UnityEngine.Random.Range(2, 10);
					if (this.parent)
					{
						w = 0.01f;
					}
					else
					{
						w = 1.5f;
					}
				}
				yield return new WaitForSeconds(w);
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x0000498B File Offset: 0x00002B8B
		public void SetTarget(GameObject g)
		{
			this.target = g.transform;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x0000499C File Offset: 0x00002B9C
		public void Update()
		{
			if (!this.dying)
			{
				if (this.parent)
				{
					if (!this.boosting)
					{
						base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.dood, this.turnValue * Time.deltaTime);
					}
					if (!this.parent)
					{
						base.transform.Translate(Vector3.right * this.speed * Time.deltaTime);
					}
					else
					{
						base.transform.position = Vector3.Lerp(base.transform.position, this.parent.transform.position, 3f * Time.deltaTime);
					}
				}
				else if (Network.isServer)
				{
					if (!this.boosting)
					{
						base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.dood, this.turnValue * Time.deltaTime);
					}
					if (!this.parent)
					{
						base.transform.Translate(Vector3.right * this.speed * Time.deltaTime);
					}
					else
					{
						base.transform.position = Vector3.Lerp(base.transform.position, this.parent.transform.position, 0.3f * Time.deltaTime);
					}
				}
			}
		}


		// Token: 0x06000064 RID: 100 RVA: 0x00004B4C File Offset: 0x00002D4C
		[RPC]
		public void TD(float[] pack)
		{
			int num = (int)pack[0];
			if (this.isMainHead)
			{
				if (!this.dying)
				{
					base.GetComponent<NetworkView>().RPC("TDTEXT", RPCMode.All, new object[]
					{
					num
					});
					this.hp -= num;
					if (this.hp <= 0)
					{
						this.dying = true;
						this.DropItems();
						if (this.isMech)
						{
							Camera.main.GetComponent<NetworkView>().RPC("dVict4", RPCMode.All, new object[0]);
						}
						base.GetComponent<NetworkView>().RPC("Hide", RPCMode.Others, new object[0]);
						base.GetComponent<NetworkView>().RPC("Uru", RPCMode.All, new object[0]);
						base.StartCoroutine(this.Die());
					}
				}
			}
			else
			{
				this.mainHead.SendMessage("TD", pack);
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00002341 File Offset: 0x00000541
		public void DropItems()
		{
			base.GetComponent<NetworkView>().RPC("DropLocal", RPCMode.All, new object[0]);
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00004C34 File Offset: 0x00002E34
		[RPC]
		public void DropLocal()
		{
			GameScript.record[SpawnerScript.curBiome]++;
			if (this.enemyID > 0)
			{
				Camera.main.SendMessage("EnemyID", this.enemyID);
			}
			if (this.enemyID != 0 && UnityEngine.Random.Range(0, 75) == 0)
			{
				Camera.main.SendMessage("AUDSPEC", SendMessageOptions.DontRequireReceiver);
				int[] array = new int[11];
				array[0] = 2500 + this.enemyID;
				array[1] = 1;
				int[] value = array;
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
				gameObject.SendMessage("InitL", value);
			}
			if (this.isLava)
			{
				if (this.isMech)
				{
					int[] array2 = new int[11];
					array2[0] = 52;
					array2[1] = UnityEngine.Random.Range(500, 2000) + GameScript.challengeLevel * 50;
					int[] value2 = array2;
					GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
					gameObject2.SendMessage("InitL", value2);
					int[] array3 = new int[11];
					array3[0] = 57;
					array3[1] = UnityEngine.Random.Range(500, 1500) + GameScript.challengeLevel * 50;
					int[] value3 = array3;
					GameObject gameObject3 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
					gameObject3.SendMessage("InitL", value3);
				}
				else
				{
					int[] array4 = new int[11];
					array4[0] = 52;
					array4[1] = UnityEngine.Random.Range(500, 2000) + GameScript.challengeLevel * 50;
					int[] value4 = array4;
					GameObject gameObject4 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
					gameObject4.SendMessage("InitL", value4);
					int[] array5 = new int[11];
					array5[0] = 57;
					array5[1] = UnityEngine.Random.Range(500, 1500) + GameScript.challengeLevel * 50;
					int[] value5 = array5;
					GameObject gameObject5 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
					gameObject5.SendMessage("InitL", value5);
				}
			}
			else
			{
				int[] array6 = new int[11];
				array6[0] = 52;
				array6[1] = UnityEngine.Random.Range(50, 200) + GameScript.challengeLevel * 50;
				int[] value6 = array6;
				GameObject gameObject6 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
				gameObject6.SendMessage("InitL", value6);
				int[] array7 = new int[11];
				array7[0] = 57;
				array7[1] = UnityEngine.Random.Range(50, 150) + GameScript.challengeLevel * 50;
				int[] value7 = array7;
				GameObject gameObject7 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
				gameObject7.SendMessage("InitL", value7);
			}
			while (this.exp > 0)
			{
				if (this.exp - 1000 > 0)
				{
					this.exp -= 1000;
					UnityEngine.Object.Instantiate(Resources.Load("exp/exp7"), this.t.position, Quaternion.identity);
				}
				else if (this.exp - 250 > 0)
				{
					this.exp -= 250;
					UnityEngine.Object.Instantiate(Resources.Load("exp/exp6"), this.t.position, Quaternion.identity);
				}
				else if (this.exp - 60 > 0)
				{
					this.exp -= 60;
					UnityEngine.Object.Instantiate(Resources.Load("exp/exp5"), this.t.position, Quaternion.identity);
				}
				else if (this.exp - 20 > 0)
				{
					this.exp -= 20;
					UnityEngine.Object.Instantiate(Resources.Load("exp/exp4"), this.t.position, Quaternion.identity);
				}
				else if (this.exp - 15 > 0)
				{
					this.exp -= 15;
					UnityEngine.Object.Instantiate(Resources.Load("exp/exp3"), this.t.position, Quaternion.identity);
				}
				else if (this.exp - 10 > 0)
				{
					this.exp -= 10;
					UnityEngine.Object.Instantiate(Resources.Load("exp/exp2"), this.t.position, Quaternion.identity);
				}
				else if (this.exp - 5 > 0)
				{
					this.exp -= 5;
					UnityEngine.Object.Instantiate(Resources.Load("exp/exp1"), this.t.position, Quaternion.identity);
				}
				else
				{
					this.exp--;
					UnityEngine.Object.Instantiate(Resources.Load("exp/exp0"), this.t.position, Quaternion.identity);
				}
			}
			if (!this.isLava)
			{
				if (this.isMykonogre)
				{
					if (UnityEngine.Random.Range(0, 20) == 0)
					{
						base.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/glitter"), Menuu.soundLevel / 10f);
						int[] array8 = new int[11];
						array8[0] = 790;
						array8[1] = 1;
						array8[3] = this.GetRandomTier();
						int[] value8 = array8;
						GameObject gameObject8 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
						gameObject8.SendMessage("InitL", value8);
						base.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/glitter"), Menuu.soundLevel / 10f);
						int[] array9 = new int[11];
						array9[0] = 890;
						array9[1] = 1;
						array9[3] = this.GetRandomTier();
						int[] value9 = array9;
						GameObject gameObject9 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
						gameObject9.SendMessage("InitL", value9);
					}
				}
				else if (this.isGlaedria)
				{
					if (UnityEngine.Random.Range(0, 20) == 0)
					{
						base.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/glitter"), Menuu.soundLevel / 10f);
						int[] array10 = new int[11];
						array10[0] = 796;
						array10[1] = 1;
						array10[3] = this.GetRandomTier();
						int[] value10 = array10;
						GameObject gameObject10 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
						gameObject10.SendMessage("InitL", value10);
						base.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/glitter"), Menuu.soundLevel / 10f);
						int[] array11 = new int[11];
						array11[0] = 896;
						array11[1] = 1;
						array11[3] = this.GetRandomTier();
						int[] value11 = array11;
						GameObject gameObject11 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
						gameObject11.SendMessage("InitL", value11);
					}
				}
				else
				{
					if (UnityEngine.Random.Range(0, 20) == 0)
					{
						base.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/glitter"), Menuu.soundLevel / 10f);
						int[] array12 = new int[11];
						array12[0] = 398;
						array12[1] = 1;
						array12[3] = this.GetRandomTier();
						int[] value12 = array12;
						GameObject gameObject12 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
						gameObject12.SendMessage("InitL", value12);
					}
					if (UnityEngine.Random.Range(0, 20) == 0)
					{
						base.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/glitter"), Menuu.soundLevel / 10f);
						int[] array13 = new int[11];
						array13[0] = 726;
						array13[1] = 1;
						array13[3] = this.GetRandomTier();
						int[] value13 = array13;
						GameObject gameObject13 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
						gameObject13.SendMessage("InitL", value13);
					}
				}
			}
			if (GameScript.challengeLevel > 0)
			{
				if (UnityEngine.Random.Range(0, 100) < GameScript.challengeLevel * 5)
				{
					Camera.main.SendMessage("AUDSPEC2", SendMessageOptions.DontRequireReceiver);
					int[] array14 = new int[11];
					array14[0] = UnityEngine.Random.Range(201, 225);
					array14[1] = 1;
					int[] value14 = array14;
					GameObject gameObject14 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
					gameObject14.SendMessage("InitL", value14);
				}
				if (UnityEngine.Random.Range(0, 100) < GameScript.challengeLevel * 3)
				{
					Camera.main.SendMessage("AUDSPEC3", SendMessageOptions.DontRequireReceiver);
					int[] array15 = new int[11];
					array15[0] = UnityEngine.Random.Range(86, 89);
					array15[1] = 1;
					int[] value15 = array15;
					GameObject gameObject15 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
					gameObject15.SendMessage("InitL", value15);
				}
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00005574 File Offset: 0x00003774
		public int GetRandomTier()
		{
			int num = UnityEngine.Random.Range(0, 100);
			num -= GameScript.challengeLevel * 2;
			int result;
			if (num < 1)
			{
				result = 3;
			}
			else if (num < 6)
			{
				result = 2;
			}
			else if (num < 16)
			{
				result = 1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000055C4 File Offset: 0x000037C4
		[RPC]
		public void Hide()
		{
			this.dying = true;
			base.transform.position = new Vector3(this.t.position.x, this.t.position.y, -500f);
			for (int i = 1; i < 7; i++)
			{
				this.wormPart[i].transform.position = new Vector3(this.t.position.x, this.t.position.y, -500f);
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x0000233F File Offset: 0x0000053F
		public void K()
		{
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00005668 File Offset: 0x00003868
		[RPC]
		public void TDTEXT(int a)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("txt"), base.transform.position, Quaternion.Euler(0f, 0f, 0f));
			gameObject.SendMessage("Init", a);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000056BC File Offset: 0x000038BC
		public IEnumerator Die()
		{
			yield return new WaitForSeconds(0.5f);
			base.transform.position = new Vector3(this.t.position.x, this.t.position.y, -500f);
			yield return new WaitForSeconds(1f);
			for (int i = 1; i < 7; i++)
			{
				if (this.wormPart[i])
				{
					this.wormPart[i].transform.position = new Vector3(this.wormPart[i].transform.position.x, this.wormPart[i].transform.position.y, -500f);
				}
			}
			yield return new WaitForSeconds(3f);
			this.wormDisassemble.SendMessage("Die");
			yield break;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x000056D8 File Offset: 0x000038D8
		public void Exile()
		{
			for (int i = 1; i < 7; i++)
			{
				Network.RemoveRPCs(this.wormPart[i].GetComponent<NetworkView>().viewID);
				Network.Destroy(this.wormPart[i]);
			}
			Menuu.characterStat[9]++;
			Network.RemoveRPCs(base.GetComponent<NetworkView>().viewID);
			Network.Destroy(base.gameObject);
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00005748 File Offset: 0x00003948
		public IEnumerator Boost()
		{
			this.boosting = true;
			this.speed = 25f;
			yield return new WaitForSeconds(0.2f);
			this.speed = 15f;
			this.boosting = false;
			yield break;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00005764 File Offset: 0x00003964
		public IEnumerator BoostMyk()
		{
			for (; ; )
			{
				if (this.hp < 1000000)
				{
					this.boosting = true;
					this.speed = 26f;
					yield return new WaitForSeconds(0.7f);
					this.speed = 18f;
					this.boosting = false;
				}
				else
				{
					this.boosting = true;
					this.speed = 22f;
					yield return new WaitForSeconds(0.7f);
					this.speed = 10f;
					this.boosting = false;
				}
				yield return new WaitForSeconds((float)UnityEngine.Random.Range(2, 4));
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00005780 File Offset: 0x00003980
		public void Wipe()
		{
			if (Network.isServer && this.isMainHead && !this.dying)
			{
				this.dying = true;
				base.GetComponent<NetworkView>().RPC("Hide", RPCMode.Others, new object[0]);
				base.StartCoroutine(this.Die());
			}
		}

		// Token: 0x0400005B RID: 91
		public GameObject mykOrb;

		// Token: 0x0400005C RID: 92
		public GameObject mykMeteor;

		// Token: 0x0400005D RID: 93
		public Material body1;

		// Token: 0x0400005E RID: 94
		public Material head1;

		// Token: 0x0400005F RID: 95
		public GameObject bodyp;

		// Token: 0x04000060 RID: 96
		public GameObject headp;

		// Token: 0x04000061 RID: 97
		public bool isMech;

		// Token: 0x04000062 RID: 98
		public bool isMykonogre;

		// Token: 0x04000063 RID: 99
		private int enemyID = 6;

		// Token: 0x04000064 RID: 100
		public bool isLava;

		// Token: 0x04000065 RID: 101
		public bool isGlaedria;

		// Token: 0x04000066 RID: 102
		public GameObject wormDisassemble;

		// Token: 0x04000067 RID: 103
		public bool isMainHead;

		// Token: 0x04000068 RID: 104
		public int exp = 110;

		// Token: 0x04000069 RID: 105
		public GameObject mainHead;

		// Token: 0x0400006A RID: 106
		public GameObject[] wormPart = new GameObject[5];

		// Token: 0x0400006B RID: 107
		public bool isHead;

		// Token: 0x0400006C RID: 108
		public GameObject parent;

		// Token: 0x0400006D RID: 109
		private Transform target;

		// Token: 0x0400006E RID: 110
		private Vector3 pos;

		// Token: 0x0400006F RID: 111
		private Vector3 dir;

		// Token: 0x04000070 RID: 112
		private float angle;

		// Token: 0x04000071 RID: 113
		private Quaternion dood;

		// Token: 0x04000072 RID: 114
		private float speed = 15f;

		// Token: 0x04000073 RID: 115
		private bool boosting;

		// Token: 0x04000074 RID: 116
		private float turnValue;

		// Token: 0x04000075 RID: 117
		private int hp = 300;

		// Token: 0x04000076 RID: 118
		private bool dying;

		// Token: 0x04000077 RID: 119
		private Rigidbody r;

		// Token: 0x04000078 RID: 120
		private Transform t;
	}

}
