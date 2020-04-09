import React from "react"

import styles from "./content-page.module.scss"

export const ContentPage = ({ children, sideBar }) => (
	<div className={styles.contentPage}>
		<div className={styles.page}>
			<div className={styles.content}>{children}</div>
			<div className={styles.sideBar}>{sideBar}</div>
		</div>
	</div>
)
