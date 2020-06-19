import React from 'react'
import PropTypes from 'prop-types'
import styles from './container.module.scss'

export const Container = ({ title, content }) =>
	<div className={styles.containerWrapper}>
		<div className={styles.container}>
			<h1 className={styles.title}>{title}</h1>
			{content}
		</div>
	</div>

Container.propTypes = {
	title: PropTypes.string,
	content: PropTypes.element,
}