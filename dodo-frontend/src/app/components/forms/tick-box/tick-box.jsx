import React from 'react'
import PropTypes from 'prop-types'
import styles from './tick-box.module.scss'

export const TickBox = ({ id, name, value, setValue }) =>
	<div className={styles.tickBox}>
		<label htmlFor={id}><h3>{name}</h3></label>
		<div className={styles.box}>
			<input
				type="checkbox"
				id={id}
				checked={value}
				onChange={e => setValue(e.target.checked)}
			/>
		</div>
	</div>

TickBox.propTypes = {
	id: PropTypes.string,
	name: PropTypes.string,
	value: PropTypes.string,
	setValue: PropTypes.func,
}