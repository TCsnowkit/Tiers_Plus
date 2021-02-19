using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiersPlus
{
	using System;
	using System.Collections;
	using UnityEngine;

	// Token: 0x02000240 RID: 576
	public class TestProjScript : EnemyScript
	{
		// Token: 0x060010D7 RID: 4311 RVA: 0x0009C4A4 File Offset: 0x0009A6A4
		public void Awake()
		{
			this.b.GetComponent<Animation>()["i"].speed = 1f;
			if (Network.isServer)
			{
				base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 3f, 0f);
				base.StartCoroutine(this.Charge());
				base.StartCoroutine(this.Shoot());
			}
			this.drops = new int[]
			{
			0,
			23,
			23
			};
			base.Initialize(200, 4, 200, this.drops, 150);
			this.networkR2 = (NetworkR2)base.gameObject.GetComponent("NetworkR2");

		}
		public void Start()
        {
			TiersPlus.GetSingleton().Logger.LogConsole("test");
			if (Network.isServer)
			{
				TiersPlus.GetSingleton().Logger.LogConsole("test2");
				this.r.useGravity = true;
			}
		}
		// Token: 0x060010D8 RID: 4312 RVA: 0x0009C588 File Offset: 0x0009A788
		[RPC]
		public void MakeFace()
		{
			this.b.GetComponent<Animation>().Play("a");
			base.GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Au/sponge"), Menuu.soundLevel / 10f);
		}

		// Token: 0x060010D9 RID: 4313 RVA: 0x0009C5C8 File Offset: 0x0009A7C8
		public IEnumerator Shoot()
		{
			for (; ; )
			{
				if (this.target)
				{
					if (Vector3.Distance(this.target.transform.position, this.t.position) < 40f)
					{
						this.shooting = true;
						base.GetComponent<NetworkView>().RPC("MakeFace", RPCMode.All, new object[0]);
						yield return new WaitForSeconds(1f);
						GameObject p = (GameObject)Network.Instantiate(Resources.Load("proj/spongeProj"), this.t.position, Quaternion.identity, 0);
						p.SendMessage("EnemySet", this.target.transform.position, SendMessageOptions.DontRequireReceiver);
						if (this.target.transform.position.x > this.t.position.x)
						{
							this.r.velocity = new Vector3(-10f, this.r.velocity.y, 0f);

						}
						else
						{
							this.r.velocity = new Vector3(10f, this.r.velocity.y, 0f);
						}
						yield return new WaitForSeconds(1.5f + (float)UnityEngine.Random.Range(-1, 2));
						this.shooting = false;
					}
					else
					{
						this.target = null;
					}
				}
				yield return new WaitForSeconds(1f);
			}
		}

		// Token: 0x060010DA RID: 4314 RVA: 0x0009C5E4 File Offset: 0x0009A7E4
		public IEnumerator Charge()
		{
			for (; ; )
			{
				if (this.target)
				{
					yield return new WaitForSeconds(0.5f);
					this.charging = true;
					try
					{
						if (this.t.position.y < this.target.transform.position.y)
						{
							this.r.velocity = new Vector3(this.r.velocity.x, 0f, 0f);
						}
						if (this.target.transform.position.x > this.t.position.x)
						{
							this.dir = 1;
						}
						else
						{
							this.dir = 0;
						}
					}
					catch
					{
						this.target = null;
					}
					yield return new WaitForSeconds(0.5f);
					try
					{
						if (this.t.position.y + 8f < this.target.transform.position.y)
						{
							this.r.velocity = new Vector3(this.r.velocity.x, 0f, 0f);
						}
					}
					catch
					{
						this.target = null;
					}
				}
				else
				{
					this.dir = 2;
				}
				yield return new WaitForSeconds(0.5f);
				this.charging = false;
			}
		}

		// Token: 0x060010DB RID: 4315 RVA: 0x0009C600 File Offset: 0x0009A800
		public void Update()
		{
			if (Network.isServer)
			{
				if (this.target && !this.knocking && !this.dead)
				{
					if (this.dir == 1)
					{
						if (this.charging && !this.shooting)
						{
							this.networkR2.mode = 0;
							this.e.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
							this.r.velocity = new Vector3(-5f, this.r.velocity.y, 0f);
						}
						else
						{
							this.networkR2.mode = 0;
						}
					}
					else if (this.dir == 0)
					{
						if (this.charging && !this.shooting)
						{
							this.networkR2.mode = 0;
							this.e.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
							this.r.velocity = new Vector3(5f, this.r.velocity.y, 0f);
						}
						else
						{
							this.networkR2.mode = 0;
						}
					}
					else
					{
						this.networkR2.mode = 0;
					}
				}
				if (this.r.velocity.y < -20f)
				{
					this.r.velocity = new Vector3(this.r.velocity.x, -19f, 0f);
				}
			}
		}

		// Token: 0x04000F3D RID: 3901
		public bool isShroomy;

		// Token: 0x04000F3E RID: 3902
		private bool charging;

		// Token: 0x04000F3F RID: 3903
		private int dir = 2;

		// Token: 0x04000F40 RID: 3904
		private bool shooting;
	}

}
