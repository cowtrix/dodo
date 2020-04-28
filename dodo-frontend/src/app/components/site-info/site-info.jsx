import React from "react"

import { SiteMap } from "app/components/site-map"
import { RebellionInfo } from "./rebellion-info"
import styles from "./site-info.module.scss"

export const SiteInfo = ({ site }) => {
	const rebellionId = site.parent ? site.parent.guid : null

	return (
		<div className={styles.siteInfo}>
			<SiteMap sites={[site]} />
			<div className={styles.address}>
				<div>Address 1</div>
				<div>Line 2</div>
				<div>Glasgow</div>
				<div>G1 1AB</div>
			</div>
			{rebellionId && <RebellionInfo rebellionId={rebellionId} />}
		</div>
	)
}
