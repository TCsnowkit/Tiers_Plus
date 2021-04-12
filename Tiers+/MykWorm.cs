using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiersPlus
{
	using System;
	using System.Collections;
	using UnityEngine;

	// Token: 0x02000026 RID: 38
	public class MykWormScript : MonoBehaviour
	{
		// Token: 0x06000110 RID: 272 RVA: 0x00009484 File Offset: 0x00007684
		public void Set(GameObject g)
		{
			this.spawner = g;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x0000948D File Offset: 0x0000768D
		[RPC]
		public void Au()
		{
			base.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/projspid"), Menuu.soundLevel / 10f);
		}

		// Token: 0x06000112 RID: 274 RVA: 0x000094B4 File Offset: 0x000076B4
		public void Awake()
		{
			if (this.isMainHead && !this.isSpider && !this.isAncient && !this.isFrozen && GameScript.challengeLevel > 0)
			{
				this.face.GetComponent<Renderer>().material = this.newFace;
			}
			if (this.isSpider)
			{
				if (GameScript.challengeLevel > 0)
				{
					if (!this.isProjSpider)
					{
						this.spiderbody.GetComponent<Renderer>().material = this.spiderbody1;
						this.speed = 7f + (float)GameScript.challengeLevel * 1.5f;
					}
					else
					{
						this.speed = 5f + (float)GameScript.challengeLevel * 1.2f;
					}
				}
				this.hp = 20;
				this.exp = 25;
			}
			else if (this.isAncient)
			{
				if (!this.isMainHead && GameScript.challengeLevel > 0)
				{
					this.desertface.GetComponent<Renderer>().material = this.wormDesert1;
				}
				this.speed = 10f;
				this.hp = 240;
				this.exp = 100;
			}
			else if (this.isFrozen)
			{
				if (!this.isMainHead && GameScript.challengeLevel > 0)
				{
					this.desertface.GetComponent<Renderer>().material = this.wormDesert1;
				}
				this.speed = 15f;
				this.hp = 10000;
				this.exp = 2000;
			}
			else if (this.isLavadragon)
			{
				this.speed = 10f;
				this.hp = 240;
				this.exp = 100;
			}
			else
			{
				this.speed += (float)GameScript.challengeLevel * 1.5f;
			}
			this.hp += GameScript.challengeLevel * 100;
			this.exp += GameScript.challengeLevel * 50;
			this.startingPos = base.transform.position;
			this.t = base.transform;
			this.r = base.GetComponent<Rigidbody>();
			if (this.parent)
			{
				base.StartCoroutine(this.UpdateTarget());
			}
			else if (Network.isServer)
			{
				base.StartCoroutine(this.UpdateTarget());
			}
		}

		// Token: 0x06000113 RID: 275 RVA: 0x0000971C File Offset: 0x0000791C
		public IEnumerator UpdateTarget()
		{
			float w = 1f;
			if (this.parent)
			{
				w = 0.1f;
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
					Vector3 position = new Vector3(this.target.position.x + (float)UnityEngine.Random.Range(-2, 3), this.target.position.y + (float)UnityEngine.Random.Range(-2, 3), 0f);
					this.dir = Camera.main.WorldToScreenPoint(position) - this.pos;
				}
				else
				{
					this.dir = this.startingPos - this.pos;
				}
				this.angle = Mathf.Atan2(this.dir.y, this.dir.x) * 57.29578f;
				this.dood = Quaternion.AngleAxis(this.angle, Vector3.forward);
				if (this.isProjSpider && this.target != null)
				{
					base.GetComponent<NetworkView>().RPC("Au", RPCMode.All, new object[0]);
					GameObject gameObject = (GameObject)Network.Instantiate(this.projspider, this.t.position, Quaternion.identity, 0);
					gameObject.SendMessage("EnemySet", this.target.position, SendMessageOptions.DontRequireReceiver);
				}
				this.turnValue = (float)UnityEngine.Random.Range(2, 6);
				if (this.isProjSpider)
				{
					yield return new WaitForSeconds(2f);
				}
				else
				{
					yield return new WaitForSeconds(w);
				}
			}
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00009737 File Offset: 0x00007937
		public void SetTarget(GameObject g)
		{
			this.target = g.transform;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00009748 File Offset: 0x00007948
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
						base.transform.position = Vector3.Lerp(base.transform.position, this.parent.transform.position, 5f * Time.deltaTime);
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
						if (this.isSpider)
						{
							if (this.target)
							{
								if (!this.knocking)
								{
									base.transform.Translate(Vector3.right * this.speed * Time.deltaTime);
								}
								else
								{
									base.transform.Translate(Vector3.left * (this.speed * 1.5f) * Time.deltaTime);
								}
							}
						}
						else
						{
							base.transform.Translate(Vector3.right * this.speed * Time.deltaTime);
						}
					}
					else
					{
						base.transform.position = Vector3.Lerp(base.transform.position, this.parent.transform.position, 0.6f * Time.deltaTime);
					}
				}
			}
			else
			{
				base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, -500f);
			}
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00009990 File Offset: 0x00007B90
		public IEnumerator Kn()
		{
			this.knocking = true;
			yield return new WaitForSeconds(1f);
			this.knocking = false;
			yield break;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x000099AC File Offset: 0x00007BAC
		[RPC]
		public void TD(float[] pack)
		{
			base.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/acube"), Menuu.soundLevel / 10f);
			int num = (int)pack[0];
			if (this.isSpider)
			{
				base.StartCoroutine(this.Kn());
			}
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
						this.DropItems();
						this.dying = true;
						base.GetComponent<NetworkView>().RPC("Hide", RPCMode.Others, new object[0]);
						base.StartCoroutine(this.Die());
					}
				}
			}
			else
			{
				this.mainHead.SendMessage("TD", pack);
			}
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00009A94 File Offset: 0x00007C94
		[RPC]
		public void Hide()
		{
			this.dying = true;
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, -500f);
			if (!this.isSpider)
			{
				for (int i = 1; i < 5; i++)
				{
					this.wormPart[i].transform.position = new Vector3(this.wormPart[i].transform.position.x, this.wormPart[i].transform.position.y, -500f);
				}
			}
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00009B54 File Offset: 0x00007D54
		[RPC]
		public void TDTEXT(int a)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("txt"), base.transform.position, Quaternion.Euler(0f, 0f, 0f));
			gameObject.SendMessage("Init", a);
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00009BA8 File Offset: 0x00007DA8
		public IEnumerator Die()
		{
			yield return new WaitForSeconds(0.5f);
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, -500f);
			if (!this.isSpider)
			{
				for (int i = 1; i < 5; i++)
				{
					if (this.wormPart[i])
					{
						this.wormPart[i].transform.position = new Vector3(this.wormPart[i].transform.position.x, this.wormPart[i].transform.position.y, -500f);
					}
				}
			}
			yield return new WaitForSeconds(3f);
			if (this.isSpider)
			{
				this.Exile();
			}
			else
			{
				this.wormDisassemble.SendMessage("Die");
			}
			yield break;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00009BC4 File Offset: 0x00007DC4
		public void Exile()
		{
			if (!this.isSpider)
			{
				for (int i = 1; i < 5; i++)
				{
					if (this.wormPart[i])
					{
						Network.RemoveRPCs(this.wormPart[i].GetComponent<NetworkView>().viewID);
						Network.Destroy(this.wormPart[i].gameObject);
					}
				}
			}
			Network.RemoveRPCs(base.GetComponent<NetworkView>().viewID);
			Network.Destroy(base.gameObject);
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00009C44 File Offset: 0x00007E44
		public IEnumerator Boost()
		{
			this.boosting = true;
			this.speed = 25f;
			yield return new WaitForSeconds(0.2f);
			this.speed = 15f;
			this.boosting = false;
			yield break;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00002341 File Offset: 0x00000541
		public void DropItems()
		{
			base.GetComponent<NetworkView>().RPC("DropLocal", RPCMode.All, new object[0]);
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00009C60 File Offset: 0x00007E60
		[RPC]
		public void DropLocal()
		{
			GameScript.record[SpawnerScript.curBiome]++;
			if (this.enemyID != 0)
			{
				Camera.main.SendMessage("EnemyID", this.enemyID);
				if (UnityEngine.Random.Range(0, 75) == 0)
				{
					Camera.main.SendMessage("AUDSPEC", SendMessageOptions.DontRequireReceiver);
					int[] array = new int[11];
					array[0] = 2500 + this.enemyID;
					array[1] = 1;
					int[] value = array;
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
					gameObject.SendMessage("InitL", value);
				}
			}
			if (this.isAncient)
			{
				int[] array2 = new int[11];
				array2[0] = 52;
				array2[1] = UnityEngine.Random.Range(50, 150) + GameScript.challengeLevel * 50;
				int[] value2 = array2;
				GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
				gameObject2.SendMessage("InitL", value2);
			}
			else
			{
				int[] array3 = new int[11];
				array3[0] = 52;
				array3[1] = UnityEngine.Random.Range(10, 80) + GameScript.challengeLevel * 50;
				int[] value3 = array3;
				GameObject gameObject3 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
				gameObject3.SendMessage("InitL", value3);
			}
			if (this.isAncient)
			{
				int[] array4 = new int[11];
				array4[0] = 23;
				array4[1] = UnityEngine.Random.Range(1, 4);
				int[] value4 = array4;
				GameObject gameObject4 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
				gameObject4.SendMessage("InitL", value4);
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
			if (GameScript.challengeLevel > 0)
			{
				if (UnityEngine.Random.Range(0, 100) < GameScript.challengeLevel * 4)
				{
					Camera.main.SendMessage("AUDSPEC2", SendMessageOptions.DontRequireReceiver);
					int[] array5 = new int[11];
					array5[0] = UnityEngine.Random.Range(201, 221);
					array5[1] = 1;
					int[] value5 = array5;
					GameObject gameObject5 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
					gameObject5.SendMessage("InitL", value5);
				}
				if (UnityEngine.Random.Range(0, 100) < GameScript.challengeLevel * 2)
				{
					Camera.main.SendMessage("AUDSPEC3", SendMessageOptions.DontRequireReceiver);
					int[] array6 = new int[11];
					array6[0] = UnityEngine.Random.Range(86, 89);
					array6[1] = 1;
					int[] value6 = array6;
					GameObject gameObject6 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("i3"), this.t.position, Quaternion.identity);
					gameObject6.SendMessage("InitL", value6);
				}
			}
		}

		// Token: 0x0600011F RID: 287 RVA: 0x0000A134 File Offset: 0x00008334
		public void Wipe()
		{
			if (Network.isServer && this.isMainHead && !this.dying)
			{
				this.dying = true;
				base.GetComponent<NetworkView>().RPC("Hide", RPCMode.Others, new object[0]);
				base.StartCoroutine(this.Die());
			}
		}

		// Token: 0x04000118 RID: 280
		public GameObject desertface;

		// Token: 0x04000119 RID: 281
		public Material wormDesert1;

		// Token: 0x0400011A RID: 282
		public GameObject projspider;

		// Token: 0x0400011B RID: 283
		public bool isProjSpider;

		// Token: 0x0400011C RID: 284
		public GameObject spiderbody;

		// Token: 0x0400011D RID: 285
		public Material spiderbody1;

		// Token: 0x0400011E RID: 286
		public Material newFace;

		// Token: 0x0400011F RID: 287
		public GameObject face;

		// Token: 0x04000120 RID: 288
		public int enemyID;

		// Token: 0x04000121 RID: 289
		public bool isLavadragon;

		// Token: 0x04000122 RID: 290
		public GameObject wormDisassemble;

		// Token: 0x04000123 RID: 291
		public bool isAncient;

		// Token: 0x04000124 RID: 292
		public bool isFrozen;

		// Token: 0x04000125 RID: 293
		public bool isSpider;

		// Token: 0x04000126 RID: 294
		private Vector3 startingPos;

		// Token: 0x04000127 RID: 295
		public int exp = 20;

		// Token: 0x04000128 RID: 296
		public bool isMainHead;

		// Token: 0x04000129 RID: 297
		public GameObject mainHead;

		// Token: 0x0400012A RID: 298
		public GameObject[] wormPart = new GameObject[5];

		// Token: 0x0400012B RID: 299
		public bool isHead;

		// Token: 0x0400012C RID: 300
		public GameObject parent;

		// Token: 0x0400012D RID: 301
		private Transform target;

		// Token: 0x0400012E RID: 302
		private Vector3 pos;

		// Token: 0x0400012F RID: 303
		private Vector3 dir;

		// Token: 0x04000130 RID: 304
		private float angle;

		// Token: 0x04000131 RID: 305
		private Quaternion dood;

		// Token: 0x04000132 RID: 306
		private float speed = 11f;

		// Token: 0x04000133 RID: 307
		private bool boosting;

		// Token: 0x04000134 RID: 308
		private float turnValue;

		// Token: 0x04000135 RID: 309
		private int hp = 60;

		// Token: 0x04000136 RID: 310
		private bool dying;

		// Token: 0x04000137 RID: 311
		private Rigidbody r;

		// Token: 0x04000138 RID: 312
		private Transform t;

		// Token: 0x04000139 RID: 313
		private bool knocking;

		// Token: 0x0400013A RID: 314
		private GameObject spawner;
	}

}
