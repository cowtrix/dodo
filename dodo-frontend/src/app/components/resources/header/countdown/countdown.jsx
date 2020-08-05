import React, { useState, useEffect } from 'react'
import PropTypes from 'prop-types'

import styles from './countdown.module.scss'

const generateTimeLeft = (date) => {
	const today = new Date()
	const rebellionStart = new Date(date)
	return (rebellionStart - today)
}

export const Countdown = ({ startDate, name }) => {

	const [time, setTime] = useState(Date.now())

	useEffect(() => {
		const interval = setInterval(() => setTime(Date.now()), 1000)
		return () => {
			clearInterval(interval)
		}
	}, [time])

	const timeLeft = generateTimeLeft(startDate)

	const _second = 1000
	const _minute = _second * 60
	const _hour = _minute * 60
	const _day = _hour * 24

	const days = Math.floor(timeLeft / _day)
	const hours = Math.floor((timeLeft % _day) / _hour)
	const minutes = Math.floor((timeLeft % _hour) / _minute)
	const seconds = Math.floor((timeLeft % _minute) / _second)

	return (
		startDate ? <div className={styles.countdown}>
			<h2>
				{name} begins in
			</h2>
			<h2>
				{days} days, {hours} hours, {minutes} minutes, {seconds} seconds
			</h2>
		</div> :
			null
	)
}


Countdown.propTypes = {
	startDate: PropTypes.string,
}