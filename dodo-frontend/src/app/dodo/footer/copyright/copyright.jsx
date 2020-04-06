import React from 'react'
import {Title} from "app/components/footer"
import styles from './copyright.module.scss'


import {COPYRIGHT_INFO_COPY, COPYRIGHT_INFO_TITLE} from './constants'

export const Copyright = () =>
    <div className={styles.copyright}>
        <Title title={COPYRIGHT_INFO_TITLE}/>
        <div>
            {COPYRIGHT_INFO_COPY}
        </div>
    </div>
