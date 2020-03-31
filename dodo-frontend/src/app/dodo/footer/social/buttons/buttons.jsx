import React from 'react'
import socialButtons from './library'
import { Icon } from 'app/components/icon'
import styles from './buttons.module.scss'

export const Buttons = () =>
	<ul className={styles.buttons}>
		{socialButtons.map(button =>
			<Icon key={button.icon} title={button.title} icon={button.icon} className={styles.button}/>
		)}
	</ul>

