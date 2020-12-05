import React from 'react'
import PropTypes from 'prop-types'
import styles from './tick-box.module.scss'

export const TickBox = ({ id, name, value, checked, setValue, message }) =>
	<div className={`${styles.tickBox} ${message ? styles.withMessage : ''}`}>
		{name && (
			<label htmlFor={id} className={styles.label}>
				<h3>{name}</h3>
			</label>
		)}
		<div className={styles.box}>
			<input
				type="checkbox"
				id={id}
				value={value}
				checked={checked}
				onChange={e => setValue(e.target.checked)}
			/>
		</div>
		{message && (
			<label htmlFor={id} className={styles.message}>{message}</label>
		)}
	</div>

TickBox.propTypes = {
	id: PropTypes.string,
	name: PropTypes.string,
	checked: PropTypes.bool,
	value: PropTypes.string,
	setValue: PropTypes.func,
	message: PropTypes.string,
	useAriaLabel: PropTypes.bool
}
