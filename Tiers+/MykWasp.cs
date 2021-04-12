using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiersPlus
{
	using System;
	using System.Collections;
	using UnityEngine;

	// Token: 0x02000269 RID: 617
	public class MykWaspScript : EnemyScript
	{
		// Token: 0x060011C6 RID: 4550 RVA: 0x000A1830 File Offset: 0x0009FA30
		public void Awake()
		{
			this.drops = new int[]
			{
			21,
			21,
			31
			};
			base.Initialize(11000, 99, 2000, this.drops, 3000);
		}

		// Token: 0x060011C7 RID: 4551 RVA: 0x000A18B4 File Offset: 0x0009FAB4
		public void Update()
		{
			if (Network.isServer)
			{
				if (this.target)
				{
					if (Mathf.Abs(this.target.transform.position.x - this.t.position.x) < 40f)
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
				if (this.moving && !this.knocking)
				{
					this.r.velocity = this.dir * this.spd;
				}
			}
		}

		// Token: 0x060011C8 RID: 4552 RVA: 0x000A1994 File Offset: 0x0009FB94
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
			this.spd = 5f + (float)GameScript.challengeLevel * 1.5f;
			this.dir = this.target.transform.position - this.t.position;
			this.dir.Normalize();
			this.moving = true;
			yield return new WaitForSeconds((float)UnityEngine.Random.Range(2, 5) * 0.5f);
			this.moving = false;
			yield return new WaitForSeconds(0.5f);
			this.attacking = false;
			yield break;
		}

		// Token: 0x0400100F RID: 4111
		public GameObject wing1;

		// Token: 0x04001010 RID: 4112
		public GameObject wing2;

		// Token: 0x04001011 RID: 4113
		public Material wing11;

		// Token: 0x04001012 RID: 4114
		public Material wing21;

		// Token: 0x04001013 RID: 4115
		public GameObject head;

		// Token: 0x04001014 RID: 4116
		public Material blackHead;

		// Token: 0x04001015 RID: 4117
		private bool attacking;

		// Token: 0x04001016 RID: 4118
		private Vector3 dir;

		// Token: 0x04001017 RID: 4119
		private bool moving;

		// Token: 0x04001018 RID: 4120
		private float spd;
	}

}
