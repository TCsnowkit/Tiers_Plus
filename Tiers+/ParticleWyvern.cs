using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiersPlus
{
	using System;
	using System.Collections;
	using UnityEngine;

	// Token: 0x02000281 RID: 641
	public class ParticleWyvernScript : EnemyScript
	{
		// Token: 0x0600124E RID: 4686 RVA: 0x000A3AD4 File Offset: 0x000A1CD4
		public void Awake()
		{
			base.canKnockback = true;
			base.isFlying = true;
			//TiersPlus.GetSingleton().Logger.LogConsole(transform.name);
			int trigIndex = 0;
			foreach (Transform t in transform.GetComponentsInChildren<Transform>(true))
			{
				
				GameObject g = t.gameObject;
				//TiersPlus.GetSingleton().Logger.LogConsole(t.name);
				switch (t.name)
				{
					case "e":
						base.e = g;
						break;
					case "wyvern":
						base.b = g;
						break;
					case "Cube":
						base.trig[trigIndex] = g;
						trigIndex++;
						break;
					default:
						break;
				}
			}
			this.drops = new int[]
			{
			0,
			0,
			0
			};
			base.Initialize(2700, 69, 600, this.drops, 510);
			this.b.GetComponent<Animation>()["a"].layer = 2;
			if (Network.isServer)
			{
				base.StartCoroutine(this.Bolt());
			}
		}

		// Token: 0x0600124F RID: 4687 RVA: 0x000A3B40 File Offset: 0x000A1D40
		[RPC]
		public void MakeFace(float[] timings)
		{
			base.StartCoroutine(this.MF(timings));
		}

		// Token: 0x06001250 RID: 4688 RVA: 0x000A3B50 File Offset: 0x000A1D50
		public IEnumerator MF(float[] timings)
		{
			this.b.GetComponent<Animation>().Play("a");
			yield return new WaitForSeconds(timings[0]);
			base.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/fire"), Menuu.soundLevel / 10f);
			yield return new WaitForSeconds(timings[1]);
			base.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/fire"), Menuu.soundLevel / 10f);
			yield return new WaitForSeconds(timings[2]);
			base.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/fire"), Menuu.soundLevel / 10f);
			yield break;
		}

		// Token: 0x06001251 RID: 4689 RVA: 0x000A3B6C File Offset: 0x000A1D6C
		public IEnumerator Bolt()
		{
			for (; ; )
			{
				if (this.target && Vector3.Distance(this.target.transform.position, this.t.position) < 20f)
				{
					this.shooting = true;
					float[] timings = new float[] { UnityEngine.Random.Range(0.4f, 0.12f), UnityEngine.Random.Range(0.2f, 0.6f), UnityEngine.Random.Range(0.2f, 0.4f) };
					base.GetComponent<NetworkView>().RPC("MakeFace", RPCMode.All, new object[] { timings } );
					yield return new WaitForSeconds(timings[0]);
					GameObject p = (GameObject)Network.Instantiate(Resources.Load("proj/wyvernCustom"), this.t.position, Quaternion.identity, 0);
					try
					{
						Vector3 projDir = target.transform.position - transform.position;
						projDir = Quaternion.AngleAxis(45, Vector3.forward) * projDir;
						p.SendMessage("EnemySet", transform.position + projDir, SendMessageOptions.DontRequireReceiver);
					}
					catch
					{
					}
					GameObject p2 = (GameObject)Network.Instantiate(Resources.Load("proj/wyvernCustom"), this.t.position, Quaternion.identity, 0);
					try
					{
						Vector3 projDir = target.transform.position - transform.position;
						projDir = Quaternion.AngleAxis(-45, Vector3.forward) * projDir;
						p2.SendMessage("EnemySet", transform.position + projDir, SendMessageOptions.DontRequireReceiver);
					}
					catch
					{
					}
					GameObject p3 = (GameObject)Network.Instantiate(Resources.Load("proj/wyvernCustom"), this.t.position, Quaternion.identity, 0);
					try
					{
						p3.SendMessage("EnemySet", target.transform.position, SendMessageOptions.DontRequireReceiver);
					}
					catch
					{
					}
					yield return new WaitForSeconds(timings[1]);
					GameObject p5 = (GameObject)Network.Instantiate(Resources.Load("proj/wyvernCustom"), this.t.position, Quaternion.identity, 0);
					try
					{
						Vector3 projDir = target.transform.position - transform.position;
						projDir = Quaternion.AngleAxis(15, Vector3.forward) * projDir;
						p5.SendMessage("EnemySet", transform.position + projDir, SendMessageOptions.DontRequireReceiver);
					}
					catch
					{
					}
					GameObject p6 = (GameObject)Network.Instantiate(Resources.Load("proj/wyvernCustom"), this.t.position, Quaternion.identity, 0);
					try
					{
						Vector3 projDir = target.transform.position - transform.position;
						projDir = Quaternion.AngleAxis(-15, Vector3.forward) * projDir;
						p6.SendMessage("EnemySet", transform.position + projDir, SendMessageOptions.DontRequireReceiver);
					}
					catch
					{
					}
					GameObject p7 = (GameObject)Network.Instantiate(Resources.Load("proj/wyvernCustom"), this.t.position, Quaternion.identity, 0);
					try
					{
						Vector3 projDir = target.transform.position - transform.position;
						projDir = Quaternion.AngleAxis(65, Vector3.forward) * projDir;
						p7.SendMessage("EnemySet", transform.position + projDir, SendMessageOptions.DontRequireReceiver);
					}
					catch
					{
					}
					GameObject p8 = (GameObject)Network.Instantiate(Resources.Load("proj/wyvernCustom"), this.t.position, Quaternion.identity, 0);
					try
					{
						Vector3 projDir = target.transform.position - transform.position;
						projDir = Quaternion.AngleAxis(-65, Vector3.forward) * projDir;
						p8.SendMessage("EnemySet", transform.position + projDir, SendMessageOptions.DontRequireReceiver);
					}
					catch
					{
					}
					yield return new WaitForSeconds(timings[2]);
					GameObject p9 = (GameObject)Network.Instantiate(Resources.Load("proj/wyvernCustom"), this.t.position, Quaternion.identity, 0);
					try
					{
						Vector3 projDir = target.transform.position - transform.position;
						projDir = Quaternion.AngleAxis(45, Vector3.forward) * projDir;
						p9.SendMessage("EnemySet", transform.position + projDir, SendMessageOptions.DontRequireReceiver);
					}
					catch
					{
					}
					GameObject p10 = (GameObject)Network.Instantiate(Resources.Load("proj/wyvernCustom"), this.t.position, Quaternion.identity, 0);
					try
					{
						Vector3 projDir = target.transform.position - transform.position;
						projDir = Quaternion.AngleAxis(-45, Vector3.forward) * projDir;
						p10.SendMessage("EnemySet", transform.position + projDir, SendMessageOptions.DontRequireReceiver);
					}
					catch
					{
					}
					GameObject p11 = (GameObject)Network.Instantiate(Resources.Load("proj/wyvernCustom"), this.t.position, Quaternion.identity, 0);
					try
					{
						p11.SendMessage("EnemySet", target.transform.position, SendMessageOptions.DontRequireReceiver);
					}
					catch
					{
					}
					yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.4f));
					this.shooting = false;
				}
				yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 4f));
			}
		}

		// Token: 0x06001252 RID: 4690 RVA: 0x000A3B88 File Offset: 0x000A1D88
		public void Update()
		{
			if (Network.isServer)
			{
				if (this.target)
				{
					if (Mathf.Abs(this.target.transform.position.x - this.t.position.x) < 80f)
					{
						if (!this.attacking)
						{
							this.attacking = true;
							base.StartCoroutine(this.Attack());
						}
					}
					else
					{
						this.target = null;
						this.r.velocity = new Vector3(0f, 0f, 0f);
					}
				}
				if (this.moving && !this.knocking && !this.shooting)
				{
					this.r.velocity = this.dir * this.spd;
				}
				else if (this.shooting && !this.knocking)
				{
					this.r.velocity = new Vector3(0f, 0f, 0f);
				}
			}
		}

		// Token: 0x06001253 RID: 4691 RVA: 0x000A3CAC File Offset: 0x000A1EAC
		public IEnumerator Attack()
		{
			if (this.target.transform.position.x > this.t.position.x)
			{
				this.e.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
			}
			else
			{
				this.e.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			}
			if (UnityEngine.Random.Range(0, 4) == 0)
			{
				this.spd = 3f;
				this.dir = this.t.position - this.target.transform.position;
			}
			else
			{
				this.spd = 10f;
				Vector3 a = new Vector3(this.target.transform.position.x, this.target.transform.position.y + (float)UnityEngine.Random.Range(-15, 15), 0f);
				this.dir = a - this.t.position;
			}
			this.dir.Normalize();
			this.moving = true;
			yield return new WaitForSeconds((float)UnityEngine.Random.Range(1, 3) * 0.2f);
			this.moving = false;
			yield return new WaitForSeconds(0.5f);
			this.attacking = false;
			yield break;
		}

		// Token: 0x04001082 RID: 4226
		private bool attacking;

		// Token: 0x04001083 RID: 4227
		private Vector3 dir;

		// Token: 0x04001084 RID: 4228
		private bool moving;

		// Token: 0x04001085 RID: 4229
		private float spd;

		// Token: 0x04001086 RID: 4230
		private bool shooting;
	}

}
