import React from "react"
import { Events } from "./events"
import { Distance } from "./distance"

import styles from "./filter.module.scss"

export const Filter = () => (
	<div className={styles.filter}>
		<Events />
		<Distance />
	</div>
)

Filter.propTypes = {}
