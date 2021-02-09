import React from 'react'
import PropTypes from 'prop-types'
import styles from './container.module.scss'
import { Loader } from '../../loader'

export const Container = ({ title, content, loading = false, isSubContainer }) =>
	<div className={styles.containerWrapper}>
		<div className={
			[styles.container, isSubContainer ? '' : styles.notSubContainer].join(' ')}>
			<Loader display={loading}/>
			{(title && <h1 className={styles.title}>{title}</h1>)}
			{content}
		</div>
	</div>

Container.propTypes = {
	title: PropTypes.string,
	content: PropTypes.element,
	loading: PropTypes.bool,
	isSubContainer: PropTypes.bool
}