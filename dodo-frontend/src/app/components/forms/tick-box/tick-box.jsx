import React from 'react'
import PropTypes from 'prop-types'
import styles from './tick-box.module.scss'

export const TickBox = ({ id, name, value, checked, setValue }) =>
	<div className={styles.tickBox}>
		<label htmlFor={id}><h3>{name}</h3></label>
		<div className={styles.box}>
			<input
				type="checkbox"
				id={id}
				value={value}
				checked={checked}
				onChange={e => setValue(e.target.checked)}
			/>
		</div>
	</div>

TickBox.propTypes = {
	id: PropTypes.string,
	name: PropTypes.string,
	checked: PropTypes.bool,
	value: PropTypes.string,
	setValue: PropTypes.func,
}