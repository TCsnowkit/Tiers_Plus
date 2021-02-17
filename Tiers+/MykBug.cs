using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiersPlus
{
	using System;
	using System.Collections;
	using UnityEngine;

	// Token: 0x0200023A RID: 570
	public class MykBugScript : EnemyScript
	{
		// Token: 0x060010B9 RID: 4281 RVA: 0x0009BAE4 File Offset: 0x00099CE4
		public void Start()
		{
			this.b.GetComponent<Animation>()["a"].speed = 1.5f;
			if (Network.isServer)
			{
				base.StartCoroutine(this.Charge());
			}
			this.drops = new int[]
			{
			21,
			21,
			21
			};
			base.Initialize(12000, 80, 3000, this.drops, 300);
			this.networkR2 = (NetworkR2)base.gameObject.GetComponent("NetworkR2");
		}

		// Token: 0x060010BA RID: 4282 RVA: 0x0009BBBC File Offset: 0x00099DBC
		public IEnumerator Charge()
		{
			for (; ; )
			{
				if (this.target)
				{
					yield return new WaitForSeconds(0.5f);
					this.charging = true;
					float yPower = 30f;
					try
					{
						if (this.t.position.y < this.target.transform.position.y)
						{
							this.r.velocity = new Vector3(this.r.velocity.x, yPower, 0f);
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
							this.r.velocity = new Vector3(this.r.velocity.x, yPower, 0f);
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

		// Token: 0x060010BB RID: 4283 RVA: 0x0009BBD8 File Offset: 0x00099DD8
		public void Update()
		{
			if (Network.isServer)
			{
				if (this.target && !this.knocking && !this.dead)
				{
					if (this.dir == 1)
					{
						if (this.charging)
						{
							this.networkR2.mode = 1;
							this.e.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
							this.r.velocity = new Vector3(6f, this.r.velocity.y, 0f);
						}
						else
						{
							this.networkR2.mode = 0;
						}
					}
					else if (this.dir == 0)
					{
						if (this.charging)
						{
							this.networkR2.mode = 1;
							this.e.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
							this.r.velocity = new Vector3(-6f, this.r.velocity.y, 0f);
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

		// Token: 0x04000F22 RID: 3874
		public GameObject head;

		// Token: 0x04000F23 RID: 3875
		public GameObject body;

		// Token: 0x04000F24 RID: 3876
		public GameObject leg;

		// Token: 0x04000F28 RID: 3880
		private bool charging;

		// Token: 0x04000F29 RID: 3881
		private int dir = 2;
	}

}
