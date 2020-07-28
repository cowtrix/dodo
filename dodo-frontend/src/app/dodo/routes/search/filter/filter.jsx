import React from "react"
import { Selector } from "app/components"

import { Events } from "./events"

import styles from "./filter.module.scss"

const title = "Filter results..."

export const Filter = () => (
	<Selector
		title={title}
		content={
			<div className={styles.filter}>
				<Events />
			</div>
		}
	/>
)

Filter.propTypes = {}
