import React from 'react'
import PropTypes from 'prop-types'
import styles from './input.module.scss'

export const Input = ({ name, id, type, value, setValue }) =>
	<div className={styles.inputWrapper}>
		<label htmlFor={name}><h3>{name}</h3></label>
		<input
			type={type}
			id={id}
			name={name}
			value={value}
			onChange={e => setValue(e.target.value)}
		/>
	</div>

Input.propTypes = {
	type: PropTypes.string,
	id: PropTypes.string,
	name: PropTypes.string,
	value: PropTypes.string,
	setValue: PropTypes.func,
}