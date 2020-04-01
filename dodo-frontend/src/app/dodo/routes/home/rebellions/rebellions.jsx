import React from 'react'

import styles from './rebellions.module.scss'
import {Header} from "./header"
import { EventList } from '../../../../components/event-list/event-list'

export const Rebellions = ({ rebellions }) =>
	<div className={styles.wrapper}>
		<div className={styles.rebellions}>
			<Header/>
			<EventList events={rebellions}/>
		</div>
	</div>

