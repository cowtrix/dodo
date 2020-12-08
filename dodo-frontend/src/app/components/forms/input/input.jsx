import React from 'react'
import PropTypes from 'prop-types'
import styles from './input.module.scss'

export const Input = ({ name, id, type, value, setValue, maxLength, error, message, placeholder }) =>
	<div className={styles.inputWrapper}>
		{name &&
			<label htmlFor={id} className={error ? styles.error : ''}>
				<h3>{name}</h3>
			</label>
		}
		<div className={styles.fieldWrapper}>
		<input
			onChange={e => setValue(e.target.value)}
			className={error ? styles.error : ''}
			{...{type, id, name, value, maxLength, placeholder}}
		/>
		{message && <div className={`${styles.message} ${error ? styles.error : ''}`}>{message}</div>}
	</div>
</div>


Input.propTypes = {
	type: PropTypes.string,
	id: PropTypes.string,
	name: PropTypes.string,
	placeholder: PropTypes.string,
	value: PropTypes.string,
	error: PropTypes.bool,
	setValue: PropTypes.func,
	maxLength: PropTypes.number,
	message: PropTypes.string
}
