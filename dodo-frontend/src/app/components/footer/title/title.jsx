import React from 'react'
import PropTypes from 'prop-types'
import styles from './title.module.scss'


export const Title = ({ title }) =>
    <h3 className={styles.title}>
        {title}
    </h3>

Title.propTypes = {
    title: PropTypes.string,
}