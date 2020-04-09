import React from "react"
import { Events } from "./events"

import styles from "./filter.module.scss"

export const Filter = () => (
	<div className={styles.filter}>
		<Events />
	</div>
)

Filter.propTypes = {}
