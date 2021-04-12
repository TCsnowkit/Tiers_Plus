using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiersPlus
{
	using System;
	using System.Collections;
	using UnityEngine;

	// Token: 0x0200027C RID: 636
	public class MykWormEggScript : EnemyScript
	{
		// Token: 0x06001234 RID: 4660 RVA: 0x000A34A4 File Offset: 0x000A16A4
		public void Awake()
		{
			this.wormEgg = (WormEgg)this.parent.gameObject.GetComponent("WormEgg");
			this.drops = new int[]
			{
			21,
			21,
			21
			};
			base.Initialize(1, 0, 0, this.drops, 0);
		}

		// Token: 0x06001235 RID: 4661 RVA: 0x000A34F8 File Offset: 0x000A16F8
		public IEnumerator Poo()
		{
			this.wormEgg.GetComponent<NetworkView>().RPC("Explode", RPCMode.All, new object[0]);
			yield return new WaitForSeconds(2f);
			this.wormEgg.GetComponent<NetworkView>().RPC("App", RPCMode.AllBuffered, new object[0]);
			yield break;
		}

		// Token: 0x06001236 RID: 4662 RVA: 0x000A3513 File Offset: 0x000A1713
		public void Update()
		{
			if (this.hp <= 0 && !this.ticked)
			{
				this.ticked = true;
				base.StartCoroutine(this.Poo());
			}
		}

		// Token: 0x0400106F RID: 4207
		private bool attacking;

		// Token: 0x04001070 RID: 4208
		private Vector3 dir;

		// Token: 0x04001071 RID: 4209
		private bool moving;

		// Token: 0x04001072 RID: 4210
		private float spd;

		// Token: 0x04001073 RID: 4211
		public GameObject parent;

		// Token: 0x04001074 RID: 4212
		private WormEgg wormEgg;

		// Token: 0x04001075 RID: 4213
		private bool ticked;
	}

}
