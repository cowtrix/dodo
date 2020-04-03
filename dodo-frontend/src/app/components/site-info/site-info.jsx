import React from "react"

import { RebellionInfo } from "./rebellion-info"
import styles from "./site-info.module.scss"

export const SiteInfo = ({ site }) => {
	const rebellionId = site.Parent ? site.Parent.guid : null

	return (
		<div className={styles.siteInfo}>
			{rebellionId && <RebellionInfo rebellionId={rebellionId} />}
		</div>
	)
}
