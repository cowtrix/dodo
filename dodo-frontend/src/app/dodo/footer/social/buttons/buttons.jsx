import React from 'react'
import socialButtons from './library'
import { Icon } from 'app/components/icon'
import styles from './buttons.module.scss'

export const Buttons = () =>
	<ul className={styles.buttons}>
		{socialButtons.map(button => (
			<li key={button.icon} className={styles.listItem}>
				<a href={button.link} target="_blank" rel="noopener noreferrer">
					<Icon
						title={button.title}
						icon={button.icon}
						className={styles.button}
						aria-label={button.title} />
				</a>
			</li>
		)
		)}
	</ul>

